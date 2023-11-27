using CallApp.Domain.Interfaces.Repositories.RefreshToken;
using CallApp.Domain.Interfaces.Repositories.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Interfaces
{
    public interface IUnitOfWork
    {
        IRefreshTokenRepository refreshTokenRepository { get; }
        IUserRepository userRepository { get; }
        IUserRoleRepository userRoleRepository { get; }
    }
}
