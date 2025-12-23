using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelimAPI.API.Common.Helper;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Application.Services;

namespace TelimAPI.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AttendanceController : ControllerBase
    {
        private readonly ITrainingService _trainingService;

        public AttendanceController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [HttpPost]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> AddSessionAttendance(Guid sessionId, [FromBody] List<SessionAttendanceDto> attendanceDtos)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponses.Fail<object>("Daxil edilən məlumatlar yanlışdır.", errors));
            }

            var result = await _trainingService.AddSessionAttendanceAsync(sessionId, attendanceDtos);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Davamiyyət qeydə alınarkən xəta baş verdi.",
                    result.Errors
                ));
            }
            return Ok(ApiResponses.Success<object>(
                message: "Davamiyyət uğurla qeydə alındı."
            ));
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetTrainingAttendance(Guid trainingId)
        {
            var result = await _trainingService.GetTrainingAttendancesAsync(trainingId);

            if (!result.Succeeded)
            {
                return NotFound(ApiResponses.Fail<object>(
                    message: "Davamiyyət hesabatı hazırlarkən xəta baş verdi.",
                    errors: result.Errors
                ));
            }
            return Ok(ApiResponses.Success(
                data: result.Data,
                message: "Təlimin ümumi davamiyyət hesabatı uğurla alındı."
            ));
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetHighAttendance(Guid trainingId)
        {
            Result<List<HighAttendanceDto>> result = await _trainingService.GetHighAttendanceAsync(trainingId);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Yüksək davamiyyətli iştirakçılar gətirilərkən xəta baş verdi.",
                    errors: result.Errors
                ));
            }
            return Ok(ApiResponses.Success(
                data: result.Data,
                message: "75%-dən yuxarı davamiyyəti olan iştirakçılar uğurla gətirildi."
            ));
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetLowAttendance(Guid trainingId)
        {
            var result = await _trainingService.GetLowAttendanceAsync(trainingId);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: result.Data.Any()
                    ? "Yuxari davamiyyətli iştirakçılar uğurla gətirildi."
                    : "75%-dən yuxari davamiyyəti olan tələbə tapılmadı.",
                    errors: result.Errors
                ));
            }
            return Ok(ApiResponses.Success(
                data: result.Data,
                message: result.Data.Any()
                    ? "Aşağı davamiyyətli iştirakçılar uğurla gətirildi."
                    : "75%-dən aşağı davamiyyəti olan tələbə tapılmadı."
            ));
        }

    }
}
