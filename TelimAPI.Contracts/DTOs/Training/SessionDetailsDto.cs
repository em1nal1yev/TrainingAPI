using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class SessionDetailsDto
    {
        public Guid SessionId { get; set; }
        public string Title { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string TrainingName { get; set; }
        public List<SessionParticipantDto> JoinedParticipants { get; set; }
    }
}
