using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.User
{
    public class JoinTrainingRequest
    {
        [Required]
        public Guid TrainingId { get; set; }
    }
}
