using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public class TrainingOngoingWithUsersDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public List<UserSimpleDto> Users { get; set; }
    }
    public class UserSimpleDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }
}
