using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class SessionParticipantDto
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; } 
        public bool IsJoined { get; set; } 
        public bool? IsPresent { get; set; }
    }
}
