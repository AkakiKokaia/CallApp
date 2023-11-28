using CallApp.Application.Services;
using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Repositories.RefreshToken;
using CallApp.Domain.Interfaces.Repositories.User;
using CallApp.Domain.Interfaces.Services;
using CallApp.Infrastructure.Repositories;
using CallApp.Infrastructure.Repositories.User;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure
{
    public static class ServiceRegistration
    {
        public static void AddInfrastructureLayer(this IServiceCollection services, IConfiguration configuration)
        {
            #region Services
            services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ITokenService, TokenService>();
            #endregion

            #region Repositories

            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IUserRoleRepository, UserRoleRepository>();

            #endregion

            services.AddOptions();

            services.AddDbContext<CallAppDBContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"), x =>
                {
                    x.CommandTimeout((int)TimeSpan.FromMinutes(3).TotalSeconds);
                }).LogTo(Console.WriteLine, LogLevel.Information);
            }, ServiceLifetime.Transient);
        }

        public static void AddAuthenticationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddIdentity<UserEntity, RoleEntity>(options =>
            {
                options.Lockout.AllowedForNewUsers = false;
            })
            .AddEntityFrameworkStores<CallAppDBContext>();

            services.Configure<IdentityOptions>(options =>
            {
                // Configure password requirements
                options.Password.RequireDigit = true;
                options.Password.RequireLowercase = true;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireUppercase = true;
                options.Password.RequiredLength = 8;
            });

            //services.AddScoped<ITokenService, TokenService>();
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = configuration["JwtSettings:Issuer"],
                    ValidAudience = configuration["JwtSettings:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
                };
            });
            services.AddAuthorization();
        }
    }
}
