using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class SessionAttendanceResultDto
    {
        public Guid SessionId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public List<UserAttendanceDto> Attendances { get; set; } = new();
    }
}
