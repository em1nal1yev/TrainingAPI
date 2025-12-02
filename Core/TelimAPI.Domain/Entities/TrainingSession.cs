using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class TrainingSession : BaseEntity
    {
        public Guid TrainingId { get; set; }
        public Training Training { get; set; } = null!;

        public string Title { get; set; } = null!;
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }

        public ICollection<SessionAttendance>? Attendances { get; set; }
    }
}
