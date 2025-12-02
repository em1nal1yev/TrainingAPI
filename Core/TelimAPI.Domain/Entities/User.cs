using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class User : IdentityUser<Guid>
    {
        public string? Name { get; set; }
        public string? Surname { get; set; }

        public Guid CourtId { get; set; }
        public Court? Court { get; set; }

        public Guid DepartmentId { get; set; }
        public Department? Department { get; set; }

        public ICollection<SessionAttendance>? SessionAttendances { get; set; }

        public ICollection<TrainingParticipant>? TrainingParticipants { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    }
}
