using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Application.Services;

namespace TelimAPI.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TrainingsController : ControllerBase
    {
        private readonly ITrainingService _trainingService;

        public TrainingsController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }


        [HttpGet("get-all")]

        public async Task<IActionResult> GetAll()
        {
            var trainings = await _trainingService.GetAllAsync();
            return Ok(trainings);
        }

        [HttpGet("get-by-{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var training = await _trainingService.GetByIdAsync(id);
            if (training == null) return NotFound("Training not found");
            return Ok(training);
        }
        [Authorize(Roles = "Trainer, Admin")]
        [HttpGet("expired")]
        public async Task<IActionResult> GetExpired()
        {
            var data = await _trainingService.GetExpiredAsync();
            return Ok(data);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ongoingTrainings")]
        public async Task<IActionResult> GetOngoingWithUsers()
        {
            var result = await _trainingService.GetOngoingAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("drafts")]
        public async Task<IActionResult> GetDrafts()
        {
            var result = await _trainingService.GetDraftsAsync();
            return Ok(result);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            await _trainingService.ApproveAsync(id);
            return Ok();
        }

        [Authorize(Roles ="Admin")]
        [HttpGet("ongoing")]
        public async Task<IActionResult> GetOngoing()
        {
            var result = await _trainingService.GetOngoingAsync();
            return Ok(result);
        }

        [Authorize(Roles = "Trainer, Admin")]
        [HttpPost("create-session")]
        public async Task<IActionResult> CreateTrainingSession([FromBody] TrainingSessionCreateDto dto)
        {
        

            
            var createdSession = await _trainingService.CreateSessionAsync(dto);

            
            return CreatedAtAction(
                nameof(GetTrainingSessionsByTrainingId), 
                new { trainingId = createdSession.Id },
                createdSession
            );
        }

        [HttpGet("GetSessions")]
        public async Task<IActionResult> GetTrainingSessionsByTrainingId(Guid trainingId)
        {
            var sessions = await _trainingService.GetSessionsByTrainingIdAsync(trainingId);
            if (sessions == null || !sessions.Any())
            {
                return NotFound($"Training with ID {trainingId} has no sessions or training not found.");
            }
            return Ok(sessions);
        }
        [HttpPost("{sessionId}/attendance")]
        [Authorize(Roles = "Trainer, Admin")] 
        public async Task<IActionResult> AddSessionAttendance(Guid sessionId, [FromBody] List<SessionAttendanceDto> attendanceDtos)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _trainingService.AddSessionAttendanceAsync(sessionId, attendanceDtos);
                return Ok(new { Message = "Attendance successfully recorded." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        [HttpGet("sessions/{sessionId}/details")]
        [Authorize(Roles = "Trainer, Admin")] 
        public async Task<IActionResult> GetSessionDetails(Guid sessionId)
        {
            try
            {
                var details = await _trainingService.GetSessionDetailsWithParticipantsAsync(sessionId);
                return Ok(details);
            }
            catch (Exception ex)
            {
                
                return NotFound(new { Message = ex.Message });
            }
        }

        [HttpPost("create")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Create([FromBody] TrainingCreateDto dto)
        {
            await _trainingService.CreateAsync(dto);
            return Ok("Training created successfully");
        }

        [HttpPut("update")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Update([FromBody] TrainingUpdateDto dto)
        {
            await _trainingService.UpdateAsync(dto);
            return Ok("Training uptaded successfuly");
        }

        [HttpDelete("delete{id}")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _trainingService.DeleteAsync(id);
            return Ok("training succcessfuly deleted");
        }
    }
}
