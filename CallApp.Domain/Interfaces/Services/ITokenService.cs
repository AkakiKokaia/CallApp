using CallApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Interfaces.Services
{
    public interface ITokenService
    {
        Task<(string AccessToken, string RefreshToken)> CreateReturnCredentials(UserEntity user, CancellationToken cancellationToken);
        Task<(string AccessToken, string RefreshToken)> RefreshAccessToken(string expiredAccessToken, string refreshToken, CancellationToken cancellationToken);
    }
}
