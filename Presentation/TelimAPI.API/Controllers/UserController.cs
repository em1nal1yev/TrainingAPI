using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TelimAPI.Application.DTOs.User;
using TelimAPI.Application.Services;

namespace TelimAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("trainings")]
        public async Task<IActionResult> GetUserTrainings()
        {
            var result = await _userService.GetUserTrainingsAsync();
            return Ok(result);
        }

        [HttpPost("trainings/join")]
        public async Task<IActionResult> JoinTraining([FromBody] JoinTrainingRequest request)
        {
            
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized("User is not authenticated. Please log in.");

            Guid userId = Guid.Parse(userIdClaim);

            try
            {
                await _userService.JoinTrainingAsync(request.TrainingId, userId);
                return Ok(new { Message = "Successfully joined the training." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpPost("trainings/feedback")]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
        {
            
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                return Unauthorized("User is not authenticated.");

            Guid userId = Guid.Parse(userIdClaim);

            try
            {
                await _userService.SubmitTrainingFeedbackAsync(request, userId);
                return Ok(new { Message = "Feedback submitted successfully." });
            }
            catch (Exception ex)
            {
                
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}
