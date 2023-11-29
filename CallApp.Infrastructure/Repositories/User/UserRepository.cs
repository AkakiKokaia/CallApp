using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces;
using CallApp.Domain.Interfaces.Repositories.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure.Repositories.User
{
    public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        private readonly UserManager<UserEntity> _userManager;
        private readonly RoleManager<RoleEntity> _roleManager;
        private readonly IUnitOfWork _uow;

        public UserRepository(
            UserManager<UserEntity> userManager,
            RoleManager<RoleEntity> roleManager,
            IUnitOfWork uow,
            CallAppDBContext context) : base(context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _uow = uow;
        }

        public async Task<UserEntity> ValidateUser(string email, string password)
        {
            var user = await _userManager.Users
                .Include(x => x.Roles)
                .ThenInclude(x => x.Role)
                .Where(x => x.Email == email)
                .FirstOrDefaultAsync();

            if (user != null && await _userManager.CheckPasswordAsync(user, password) && await ValidateUserAvailability(user))
            {
                return user;
            }
            return null;
        }

        public async Task<bool> ValidateUserAvailability(UserEntity user, CancellationToken cancellationToken = default)
        {
            if(user.LockoutEnabled && user.LockoutEnd > DateTime.Now)
            {
                return false;
            }
            else if(user.LockoutEnabled && user.LockoutEnd <= DateTime.Now)
            {
                user.LockoutEnabled = false;
                user.LockoutEnd = null;

                await _uow.userRepository.SaveChangesAsync(cancellationToken);
                return true;
            }
            return true;
        }

        public async Task CreateUser(UserEntity user, string password)
        {
            var result = await _userManager.CreateAsync(user, password);
            if (result.Succeeded)
            {
                var getUser = await _userManager.Users.FirstOrDefaultAsync(x => x.Email == user.Email);
                await _uow.userRoleRepository.AddToRoleAsync(getUser, 1);
            }
            else 
            {
                throw new Exception("Something went wrong");
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var entities = context.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added);

            foreach (var entity in entities)
            {
                if (entity.Entity is UserEntity updatedEntity)
                {
                    updatedEntity.CreatedAt = DateTime.Now;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }

        public override async Task<UserEntity> GetById(int Id)
        {
            return await context.Set<UserEntity>()
                .Include(x => x.UserProfiles)
                .FirstOrDefaultAsync(x => x.Id == Id);
        }
    }
}
