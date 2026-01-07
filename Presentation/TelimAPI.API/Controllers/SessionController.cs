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
    public class SessionController : BaseController
    {
        private readonly ITrainingService _trainingService;

        public SessionController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }
        [HttpPost]
        public async Task<IActionResult> CreateTrainingSession([FromBody] TrainingSessionCreateDto dto)
        {
            return await ExecuteAsync(() => _trainingService.CreateSessionAsync(dto));
        }

        [HttpGet("{trainingId}")]
        public async Task<IActionResult> GetTrainingSessionsByTrainingId(Guid trainingId)
        {
            return await ExecuteAsync(() => _trainingService.GetSessionsByTrainingIdAsync(trainingId));
        }

        [HttpGet("details/{sessionId}")]
        public async Task<IActionResult> GetSessionDetails(Guid sessionId)
        {
            return await ExecuteAsync(() => _trainingService.GetSessionDetailsWithParticipantsAsync(sessionId));
        }

    }
}
