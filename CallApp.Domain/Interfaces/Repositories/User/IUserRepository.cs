using CallApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Interfaces.Repositories.User
{
    public interface IUserRepository : IGenericRepository<UserEntity>
    {
        Task CreateUser(UserEntity user, string password);
        Task<UserEntity> ValidateUser(string email, string password);
        Task<bool> ValidateUserAvailability(UserEntity user, CancellationToken cancellationToken = default);
    }
}
