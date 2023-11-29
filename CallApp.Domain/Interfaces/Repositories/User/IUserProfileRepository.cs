using CallApp.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Interfaces.Repositories.User
{
    public interface IUserProfileRepository : IGenericRepository<UserProfileEntity>
    {
        Task CreateUserProfile(UserProfileEntity request, int id);
    }
}
