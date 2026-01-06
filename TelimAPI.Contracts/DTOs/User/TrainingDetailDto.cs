using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.User
{
    public class TrainingDetailDto
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public List<string> Courts { get; set; }
        public List<string> Departments { get; set; }
        public int ParticipantsCount { get; set; }
        public bool IsJoined { get; set; }
    }
}
