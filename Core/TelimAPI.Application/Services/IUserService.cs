using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.User;

namespace TelimAPI.Application.Services
{
    public interface IUserService
    {
        Task<Result<UserTrainingsDto>> GetUserTrainingsAsync();
        Task<Result> JoinTrainingAsync(Guid trainingId, Guid userId);

        Task<Result> SubmitTrainingFeedbackAsync(SubmitFeedbackRequest request, Guid userId);
    }
}
