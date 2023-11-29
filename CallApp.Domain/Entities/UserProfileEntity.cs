using CallApp.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Entities
{
    public class UserProfileEntity : BaseEntity
    {
        public int UserId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [StringLength(11, ErrorMessage = "IDNumber must be 11 characters long.")]
        public string IDNumber { get; set; }

        public virtual UserEntity User { get; set; }
    }
}
