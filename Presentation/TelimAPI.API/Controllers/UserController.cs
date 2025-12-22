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
    public class UserController : ControllerBase
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
            Result<UserTrainingsDto> result = await _userService.GetUserTrainingsAsync();

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Təlimlər gətirilərkən xəta baş verdi.",
                    errors: result.Errors
                ));
            }

            return Ok(ApiResponses.Success(
                data: result.Data,
                message: "İştirak etdiyiniz təlimlərin siyahısı."
            ));
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> JoinTraining([FromBody] JoinTrainingRequest request)
        {
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(ApiResponses.Fail<object>("İstifadəçi tapılmadı. Zəhmət olmasa yenidən giriş edin."));
            }

            
            Result result = await _userService.JoinTrainingAsync(request.TrainingId, userId);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Təlimə qoşulmaq mümkün olmadı.",
                    errors: result.Errors
                ));
            }

            return Ok(ApiResponses.Success<object>(
                message: "Təlimə uğurla qoşuldunuz."
            ));
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request)
        {
            
            var userIdClaim = User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out Guid userId))
            {
                return Unauthorized(ApiResponses.Fail<object>("İstifadəçi tapılmadı. Zəhmət olmasa yenidən giriş edin."));
            }

            
            Result result = await _userService.SubmitTrainingFeedbackAsync(request, userId);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Rəy göndərilərkən xəta baş verdi.",
                    errors: result.Errors
                ));
            }

            
            return Ok(ApiResponses.Success<object>(
                message: "Rəyiniz uğurla qeydə alındı. Təşəkkür edirik!"
            ));
        }
    }
}
