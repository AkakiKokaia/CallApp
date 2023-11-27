using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Entities
{
    public class UserEntity : IdentityUser<int>
    {
        public int Id { get; set; }
        public string Email { get; set; }
        public bool IsActive { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public virtual ICollection<UserProfileEntity> UserProfiles { get; set; } = new List<UserProfileEntity>();
        public virtual ICollection<UserRoleEntity> Roles { get; } = new List<UserRoleEntity>();
        public virtual ICollection<RefreshTokenEntity> RefreshTokens { get; set; } = new List<RefreshTokenEntity>();
    }
}
