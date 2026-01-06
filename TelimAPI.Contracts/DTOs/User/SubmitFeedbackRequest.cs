using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.User
{
    public class SubmitFeedbackRequest
    {
        [Required]
        public Guid TrainingId { get; set; }

        [Range(1, 5, ErrorMessage = "TrainingRating must be between 1 and 5.")]
        public int TrainingRating { get; set; }

        [Range(1, 5, ErrorMessage = "TrainerRating must be between 1 and 5.")]
        public int TrainerRating { get; set; }

        [MaxLength(500)]
        public string? Comment { get; set; }
    }
}
