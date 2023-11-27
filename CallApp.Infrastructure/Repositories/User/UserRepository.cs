using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces.Repositories.User;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure.Repositories.User
{
    public class UserRepository : GenericRepository<UserEntity>, IUserRepository
    {
        private readonly UserManager<UserEntity> _userManager;

        public UserRepository(
            UserManager<UserEntity> userManager,
            CallAppDBContext context) : base(context)
        {
            _userManager = userManager;
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
    }
}
