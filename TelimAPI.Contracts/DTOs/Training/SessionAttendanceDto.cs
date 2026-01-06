using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class SessionAttendanceDto
    {
        public Guid UserId { get; set; }
        public bool IsPresent { get; set; }
    }
}
