using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class Training : BaseEntity
    {
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime? Date { get; set; }

        public ICollection<TrainingCourt>? TrainingCourts { get; set; }
        public ICollection<TrainingDepartment>? TrainingDepartments { get; set; }

        public ICollection<TrainingParticipant>? Participants { get; set; }
    }
}
