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
    public class SessionController : ControllerBase
    {
        private readonly ITrainingService _trainingService;

        public SessionController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }

        [Authorize(Roles = "Trainer, Admin")]
        [HttpPost]
        public async Task<IActionResult> CreateTrainingSession([FromBody] TrainingSessionCreateDto dto)
        {

            Result<TrainingSessionGetDto> result = await _trainingService.CreateSessionAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Sessiya yaradılarkən xəta baş verdi",
                    result.Errors));
            }

            return CreatedAtAction(
                nameof(GetTrainingSessionsByTrainingId),
                new { trainingId = result.Data.Id },
                ApiResponses.Success(result.Data, "Sessiya uğurla yaradıldı.")
            );
        }

        [HttpGet]
        public async Task<IActionResult> GetTrainingSessionsByTrainingId(Guid trainingId)
        {
            Result<List<TrainingSessionGetDto>> result = await _trainingService.GetSessionsByTrainingIdAsync(trainingId);
            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    "Sessiyaları gətirərkən xəta baş verdi",
                    result.Errors
                ));
            }
            return Ok(ApiResponses.Success(
                result.Data,
                "Sessiyalar uğurla gətirildi."
            ));
        }
        
        [HttpGet]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> GetSessionDetails(Guid sessionId)
        {
            var result = await _trainingService.GetSessionDetailsWithParticipantsAsync(sessionId);

            if (!result.Succeeded)
            {

                return NotFound(ApiResponses.Fail<object>(
                    message: "Sessiya detalları gətirilərkən xəta baş verdi.",
                    errors: result.Errors
                ));
            }
            return Ok(ApiResponses.Success(
                data: result.Data,
                message: "Sessiya detalları uğurla alındı."
            ));
        }

    }
}
