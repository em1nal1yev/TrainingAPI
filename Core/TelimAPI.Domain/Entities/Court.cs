using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class Court : BaseEntity
    {
        public string? Name { get; set; }

        public ICollection<Department>? Departments { get; set; }
        public ICollection<User>? Users { get; set; }
        public ICollection<TrainingCourt>? TrainingCourts { get; set; }
    }
}
