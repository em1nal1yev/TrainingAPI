using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class HighAttendanceDto
    {
        public Guid UserId { get; set; }
        public string? Fullname { get; set; }
        public int TotalSessions { get; set; }
        public int AttendedSessions { get; set; }
        public double AttendanceRate { get; set; }
    }
}
