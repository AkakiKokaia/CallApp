using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Entities
{
    public class RoleEntity : IdentityRole<int>
    {
        public override string Name { get; set; }
        public override string NormalizedName { get; set; }
        public virtual ICollection<UserRoleEntity> Users { get; } = new List<UserRoleEntity>();
    }
}
