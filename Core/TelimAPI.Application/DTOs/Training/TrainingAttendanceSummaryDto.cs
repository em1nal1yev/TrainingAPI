using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class TrainingAttendanceSummaryDto
    {
        public Guid TrainingId { get; set; }
        public string TrainingTitle { get; set; } = "";
        public List<SessionAttendanceResultDto> Sessions { get; set; } = new();
    }
}
