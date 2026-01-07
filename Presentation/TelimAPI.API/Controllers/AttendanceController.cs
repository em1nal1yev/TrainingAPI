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
    public class AttendanceController : BaseController
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
            return await ExecuteAsync(() => _trainingService.AddSessionAttendanceAsync(sessionId, attendanceDtos));
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetTrainingAttendance(Guid trainingId)
        {
            return await ExecuteAsync(() => _trainingService.GetTrainingAttendancesAsync(trainingId));
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetHighAttendance(Guid trainingId)
        {
            return await ExecuteAsync(() => _trainingService.GetHighAttendanceAsync(trainingId));
        }

        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetLowAttendance(Guid trainingId)
        {
            return await ExecuteAsync(() => _trainingService.GetLowAttendanceAsync(trainingId));
        }

    }
}
