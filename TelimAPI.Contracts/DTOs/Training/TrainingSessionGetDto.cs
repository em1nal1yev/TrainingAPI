using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public record TrainingSessionGetDto(
    
        Guid Id,
        DateTime StartTime,
        DateTime EndTime,
        int TotalParticipants,
        int PresentCount

);
}
