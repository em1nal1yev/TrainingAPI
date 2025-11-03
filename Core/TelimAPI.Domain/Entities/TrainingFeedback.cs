using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities.Common;

namespace TelimAPI.Domain.Entities
{
    public class TrainingFeedback : BaseEntity
    {
        public Guid TrainingParticipantId { get; set; }
        public TrainingParticipant? TrainingParticipant { get; set; }

        public int TrainingRating { get; set; }  // 1–5
        public int TrainerRating { get; set; }   // 1–5
        public string? Comment { get; set; }
    }
}
