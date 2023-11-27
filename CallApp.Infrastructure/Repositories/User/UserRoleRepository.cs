using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces.Repositories.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure.Repositories.User
{
    internal class UserRoleRepository : GenericRepository<UserRoleEntity>, IUserRoleRepository
    {
        private readonly RoleManager<RoleEntity> _roleManager;
        private readonly IConfiguration _configuration;

        public UserRoleRepository(RoleManager<RoleEntity> roleManager, IConfiguration configuration, CallAppDBContext context) : base(context)
        {
            _roleManager = roleManager;
            _configuration = configuration;
        }
    }
}
