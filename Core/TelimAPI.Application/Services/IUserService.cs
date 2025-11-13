using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.User;

namespace TelimAPI.Application.Services
{
    public interface IUserService
    {
        Task<UserTrainingsDto> GetUserTrainingsAsync();
        Task JoinTrainingAsync(Guid trainingId, Guid userId);

        Task SubmitTrainingFeedbackAsync(SubmitFeedbackRequest request, Guid userId);
    }
}
