using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;
using TelimAPI.Domain.Enums;

namespace TelimAPI.Domain.Entities
{
    public class Training : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public TrainingStatus Status { get; set; } = TrainingStatus.Draft;
        public ICollection<TrainingSession>? Sessions { get; set; }

        public ICollection<TrainingCourt>? TrainingCourts { get; set; }
        public ICollection<TrainingDepartment>? TrainingDepartments { get; set; }

        public ICollection<TrainingParticipant>? Participants { get; set; }
    }
}
