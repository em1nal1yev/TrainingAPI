using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class TrainingParticipant : BaseEntity
    {
        public Guid TrainingId { get; set; }
        public Training Training { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsJoined { get; set; }

        
    }
}
