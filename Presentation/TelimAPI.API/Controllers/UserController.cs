using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelimAPI.API.Common.Helper;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.User;
using TelimAPI.Application.Services;

namespace TelimAPI.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : BaseController
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUserTrainings()
        {
            return await ExecuteAsync(() => _userService.GetUserTrainingsAsync());
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> JoinTraining([FromBody] JoinTrainingRequest request)
        {
            return await ExecuteAsync(() => _userService.JoinTrainingAsync(request.TrainingId, UserId));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
        {
            return await ExecuteAsync(() => _userService.SubmitTrainingFeedbackAsync(request, UserId));
        }
    }
}
