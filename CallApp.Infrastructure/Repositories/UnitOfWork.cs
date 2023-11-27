using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Repositories;
using CallApp.Domain.Interfaces.Repositories.RefreshToken;
using CallApp.Domain.Interfaces.Repositories.User;
using CallApp.Infrastructure.Repositories.User;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private IServiceProvider _serviceProvider;

        public UnitOfWork(IServiceProvider serviceProvider)
        {
           _serviceProvider = serviceProvider;
        }

        #region Repositories

        public IRefreshTokenRepository refreshTokenRepository => _serviceProvider.GetService<IRefreshTokenRepository>();
        public IUserRepository userRepository => _serviceProvider.GetService<IUserRepository>();

        #endregion
    }
}
