using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class RefreshToken:BaseEntity
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        
        public bool IsRevoked { get; set; } = false;

        
        public DateTime Expires { get; set; }

        
        public Guid UserId { get; set; }
        public User User { get; set; } = null!;
    }
}
