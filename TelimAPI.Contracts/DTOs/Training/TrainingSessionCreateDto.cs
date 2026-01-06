using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public record TrainingSessionCreateDto
    {
        [Required]
        public Guid TrainingId { get; init; } 

        [Required]
        [MaxLength(200)]
        public string Title { get; init; } = null!;

        [Required]
        public DateTime StartTime { get; init; }

        [Required]
        public DateTime EndTime { get; init; }
    }
}
