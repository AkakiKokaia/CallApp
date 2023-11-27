using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Application.Services
{
    public class TokenService : ITokenService
    {
        private readonly SymmetricSecurityKey tokenKey;
        private readonly IConfiguration configuration;
        private readonly IUnitOfWork unitOfWork;
        private readonly IHttpContextAccessor httpContextAccessor;

        public TokenService(SymmetricSecurityKey tokenKey, IConfiguration configuration, IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
        {
            this.tokenKey = tokenKey;
            this.configuration = configuration;
            this.unitOfWork = unitOfWork;
            this.httpContextAccessor = httpContextAccessor;
        }

        public async Task<(string AccessToken, string RefreshToken)> CreateReturnCredentials(UserEntity user, CancellationToken cancellationToken)
        {
            var claims = CreateUserClaims(user);

            var creds = new SigningCredentials(tokenKey, SecurityAlgorithms.HmacSha512Signature);

            var AccessTokenExpirationMinutes = configuration["JwtSettings:AccessTokenExpirationMinutes"];

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Audience = configuration["JwtSettings:Issuer"],
                Issuer = configuration["JwtSettings:Issuer"],
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(int.Parse(AccessTokenExpirationMinutes)),
                SigningCredentials = creds,
            };

            var TokenHandler = new JwtSecurityTokenHandler();

            var refreshTokens = unitOfWork.refreshTokenRepository.GetAllWhere(x => x.UserId == user.Id);

            await DisableOldRefreshTokens(user.RefreshTokens.ToList());

            var token = TokenHandler.CreateToken(tokenDescriptor);
            // Writes JWT Token
            var tokenResult = TokenHandler.WriteToken(token);
            // Creates Refresh Token
            var refreshToken = await CreateRefreshToken(user.Id);

            await SaveRefreshTokenInDb(refreshToken, user.Id, cancellationToken);

            return (tokenResult, refreshToken.RefreshToken);
        }

        public async Task<(string AccessToken, string RefreshToken)> RefreshAccessToken(string expiredAccessToken, string refreshToken, CancellationToken cancellationToken)
        {
            var claimsPrincipal = GetPrincipalFromExpiredToken(expiredAccessToken);
            var userId = claimsPrincipal.FindFirst(JwtRegisteredClaimNames.NameId).Value;
            // validate the refresh token
            var isValidRefreshToken = await ValidateRefreshToken(userId, refreshToken);
            // handle invalid refresh token
            if (!isValidRefreshToken.isValid) throw new SecurityTokenException("Invalid refresh token.");
            // generate a new access token
            var newAccessToken = await CreateReturnCredentials(isValidRefreshToken.user, cancellationToken);

            return newAccessToken;
        }

        private List<Claim> CreateUserClaims(UserEntity user)
        {
            return new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Name, user.UserName),
                new Claim(JwtRegisteredClaimNames.GivenName, user.UserProfiles.FirstOrDefault().FirstName),
                new Claim(JwtRegisteredClaimNames.FamilyName, user.UserProfiles.FirstOrDefault().LastName),
                new Claim(ClaimTypes.Role, user.Roles.FirstOrDefault().Role.Name),
            };
        }

        private async Task DisableOldRefreshTokens(List<RefreshTokenEntity> refreshTokens)
        {
            if (refreshTokens.Where(x => x.Revoked != true).Any()) 
            {
                var websender = httpContextAccessor.HttpContext.Request.Host.Value;
                refreshTokens
                    .Where(x => (x.Website == null || x.Website == websender)
                    && x.Revoked != true)
                    .ToList()
                    .ForEach(x =>
                    {
                        x.Revoked = true;
                        x.RevokedAt = DateTime.Now;
                    });
                await unitOfWork.refreshTokenRepository.UpdateRange(refreshTokens);
                await unitOfWork.refreshTokenRepository.SaveChangesAsync();
            }
        }

        private async Task<RefreshTokenEntity> CreateRefreshToken(int userId)
        {
            var refreshToken = new RefreshTokenEntity
            {
                UserId = userId,
                RefreshToken = Guid.NewGuid().ToString("N"),
                ExpirationTime = DateTime.Now.AddDays(int.Parse(configuration["JwtSettings:RefreshTokenExpirationDays"])),
                Website = httpContextAccessor.HttpContext.Request.Host.Value
            };

            // Return the refresh token
            return refreshToken;
        }

        private async Task SaveRefreshTokenInDb(RefreshTokenEntity newEntity, int userId, CancellationToken cancellationToken)
        {
            var existingTokens = await unitOfWork.refreshTokenRepository.GetAllWhereAsync(x => x.UserId == userId);
            await DisableOldRefreshTokens(existingTokens.ToList());
            await unitOfWork.refreshTokenRepository.Add(newEntity, cancellationToken);
        }

        private async Task<(bool isValid, UserEntity? user)> ValidateRefreshToken(string userId, string refreshToken)
        {
            var parsingResult = int.TryParse(userId, out int guidUserId);
            var webSender = httpContextAccessor.HttpContext.Request.Host.Value;

            if (!parsingResult) throw new KeyNotFoundException("User Id Is Incorrect");
            // Retrieve the user from the database by their ID
            var user = await unitOfWork.userRepository.FindFirst(x => x.Id == guidUserId);

            // Check if user and refreshtoken exists and sent token is in correct format
            if (user == null
                || !user.RefreshTokens.Any()
                || !user.RefreshTokens.OrderByDescending(x => x.CreatedAt).Any(x => x.RefreshToken == refreshToken && x.Website == webSender))
            {
                throw new KeyNotFoundException("Incorrect refresh token data");
            }

            if (user.RefreshTokens.Any(x => x.RefreshToken == refreshToken && x.Website == webSender)
                && user.RefreshTokens.FirstOrDefault(x => x.RefreshToken == refreshToken && x.Website == webSender).ExpirationTime < DateTime.Now)
            {
                throw new SecurityTokenException("Token validity expired!");
            }

            var userRefreshTokens = await unitOfWork.userRepository
                                                    .GetAllWhere(x => x.Id == guidUserId)
                                                    .Include(x => x.RefreshTokens.Where(x => x.Revoked != true))
                                                    .FirstOrDefaultAsync();

            await DisableOldRefreshTokens(userRefreshTokens.RefreshTokens.ToList());

            return (true, user);
        }
        private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = false,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = tokenKey
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var claims = jwtToken.Claims;

                return new ClaimsPrincipal(new ClaimsIdentity(claims));
            }
            catch (Exception)
            {
                throw new SecurityTokenException("Invalid Token");
            }
        }
    }
}
