using CallApp.Domain.Entities;
using CallApp.Domain.Entities.Common;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure
{
    public class CallAppDBContext : IdentityDbContext<UserEntity, RoleEntity, int, IdentityUserClaim<int>, IdentityUserRole<int>, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
    {
        protected readonly IConfiguration Configuration;
        protected readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IMediator _mediator;

        public CallAppDBContext(DbContextOptions<CallAppDBContext> options, IConfiguration configuration, IHttpContextAccessor httpContextAccessor, IMediator mediator)
            : base(options)
        {
            Configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            _mediator = mediator;
        }

        public DbSet<UserEntity> Users { get; set; }
        public DbSet<RoleEntity> Role { get; set; }
        public DbSet<UserRoleEntity> UserRoles { get; set; }
        public DbSet<UserProfileEntity> UserProfile { get; set; }
        public DbSet<RefreshTokenEntity> RefreshTokens { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            //builder.Entity<UserRoleEntity>()
            //    .HasKey(ur => new { ur.UserId, ur.RoleId });

            builder.Entity<UserRoleEntity>()
                .HasOne(ur => ur.User)
                .WithMany(u => u.Roles)
                .HasForeignKey(u => u.UserId);

            builder.Entity<UserRoleEntity>()
                .HasOne(u => u.Role)
                .WithMany(r => r.Users)
                .HasForeignKey(ur => ur.RoleId);

            builder.Entity<UserProfileEntity>()
                .HasOne(u => u.User)
                .WithOne()
                .OnDelete(DeleteBehavior.Cascade);

        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var entities = ChangeTracker.Entries()
            .Where(e => (e.State == EntityState.Modified || e.State == EntityState.Added) && e.Entity is BaseEntity);

            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            var claimedUserId = string.Empty;

            if (userIdClaim != null)
            {
                var userIdClaimValue = userIdClaim.Value;
                if (int.TryParse(userIdClaimValue, out int userId))
                {
                    foreach (var entity in entities)
                    {
                        if (entity.Entity is BaseEntity updatedEntity)
                        {
                            if (updatedEntity.CreatedById == null)
                            {
                                updatedEntity.CreatedById = userId;
                            }

                            updatedEntity.UpdatedById = userId;
                            updatedEntity.UpdatedAt = DateTime.Now;
                        }
                    }
                }
                else 
                {
                    throw new Exception($"Error: Unable to parse '{userIdClaimValue}' as an integer for user ID.");
                }
            }

            var returnData = await base.SaveChangesAsync(cancellationToken);

            var domainEntities = ChangeTracker
            .Entries<BaseEntity>()
            .Where(x => x.Entity.DomainEvents != null && x.Entity.DomainEvents.Any());

            var domainEvents = domainEntities
                .SelectMany(x => x.Entity.DomainEvents)
                .ToList();

            domainEntities.ToList()
                .ForEach(entity => entity.Entity.ClearDomainEvents());

            var tasks = domainEvents
                .Select(async (domainEvent) =>
                {
                    await _mediator.Publish(domainEvent, cancellationToken);
                });

            await Task.WhenAll(tasks);

            return returnData;
        }
    }
}
