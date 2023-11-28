using CallApp.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure
{
    public class DBInitializer
    {
        public static void InitializeDatabase(IServiceProvider serviceProvider, CallAppDBContext context)
        {

            #region Migrations
            using IServiceScope serviceScope = serviceProvider.CreateScope();

            try
            {
                if (context.Database.GetPendingMigrations().Any())
                {
                    context.Database.Migrate();
                }
                var initializer = new DBInitializer();
                initializer.Seed(context);
            }
            catch (Exception ex)
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<CallAppDBContext>>();



                logger.LogError(ex, "An error occurred while migrating or seeding the database.");



                throw;
            }
            #endregion
        }

        #region Seeding

        private void Seed(CallAppDBContext context)
        {
            context.Database.EnsureCreated();
            SeedRoles(context);
        }

        private void SeedRoles(CallAppDBContext context)
        {
            if (!context.Roles.Any())
            {
                RoleEntity role = new();

                role = new RoleEntity
                {
                    Id = 1,
                    Name = "User",
                    NormalizedName = "USER",
                    ConcurrencyStamp = null
                };
                context.Add(role);
                role = new RoleEntity
                {
                    Id = 2,
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                    ConcurrencyStamp = null
                };
                context.Add(role);
                context.SaveChanges();
            }
        }

        #endregion
    }
}
