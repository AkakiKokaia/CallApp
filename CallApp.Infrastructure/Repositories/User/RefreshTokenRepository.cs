using CallApp.Domain.Entities;
using CallApp.Domain.Interfaces.Repositories.RefreshToken;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Infrastructure.Repositories.User
{
    public class RefreshTokenRepository : GenericRepository<RefreshTokenEntity>, IRefreshTokenRepository
    {
        public RefreshTokenRepository(
            CallAppDBContext context) : base(context) { }
    }
}
