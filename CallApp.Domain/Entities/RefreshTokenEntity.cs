using CallApp.Domain.Entities.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CallApp.Domain.Entities
{
    public class RefreshTokenEntity : BaseEntity
    {
        public int UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual UserEntity User { get; set; }
        public string RefreshToken { get; set; }
        public DateTime ExpirationTime { get; set; }
        public bool Revoked { get; set; } = false;
        public DateTime? RevokedAt { get; set; }
        public string? Website { get; set; }
    }
}
