using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.User
{
    public class UserTrainingsDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public List<TrainingDetailDto> Trainings { get; set; }
    }
}
