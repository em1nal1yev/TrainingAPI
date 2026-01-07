using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelimAPI.API.Common.Helper;
using TelimAPI.Application.Common.Results;


namespace TelimAPI.API.Controllers
{
    [ApiController]
    public class BaseController : ControllerBase
    {
        private const string SuccessMessage = "Əməliyyat uğurla yerinə yetirildi";


        protected async Task<IActionResult> ExecuteAsync<T>(
            Func<Task<Result<T>>> action)
        {
            Result<T> result;

            try
            {
                result = await action();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponses.Fail<object>(
                    "Gözlənilməz xəta baş verdi",
                    new List<string> { ex.Message }
                ));
            }

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Əməliyyat uğursuz oldu",
                    result.Errors
                ));
            }

            return Ok(ApiResponses.Success(
                result.Data,
                SuccessMessage
            ));
        }


        protected async Task<IActionResult> ExecuteAsync(
            Func<Task<Result>> action)
        {
            Result result;

            try
            {
                result = await action();
            }
            catch (Exception ex)
            {
                return StatusCode(500, ApiResponses.Fail<object>(
                    "Gözlənilməz xəta baş verdi",
                    new List<string> { ex.Message }
                ));
            }

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Əməliyyat uğursuz oldu",
                    result.Errors
                ));
            }

            return Ok(ApiResponses.Success<object>(
                message: SuccessMessage
            ));
        }

        protected Guid UserId
        {
            get
            {
                var userIdClaim = User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userIdClaim))
                    return Guid.Empty;

                return Guid.Parse(userIdClaim);
            }
        }

    }
}
