using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class SessionAttendance : BaseEntity
    {
        public Guid TrainingSessionId { get; set; }
        public TrainingSession TrainingSession { get; set; } = null!;

        public Guid UserId { get; set; }
        public User User { get; set; } = null!;

        public bool IsPresent { get; set; } // attended or not
        public DateTime? JoinedTime { get; set; }
    }
}
