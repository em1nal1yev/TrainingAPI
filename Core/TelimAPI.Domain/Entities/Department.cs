using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class Department : BaseEntity
    {
        public string? Name { get; set; }

        public Guid CourtId { get; set; }
        public Court? Court { get; set; }

        public ICollection<User>? Users { get; set; }
        public ICollection<TrainingDepartment>? TrainingDepartments { get; set; }
    }
}
