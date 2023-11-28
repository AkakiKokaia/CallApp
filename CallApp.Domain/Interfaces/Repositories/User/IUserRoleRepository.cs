using CallApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Interfaces.Repositories.User
{
    public interface IUserRoleRepository : IGenericRepository<UserRoleEntity>
    {
        Task AddToRoleAsync(UserEntity user, int roleId);
    }
}
