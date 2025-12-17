using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TelimAPI.API.Common.Helper;
using TelimAPI.Application.Common.Results;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Application.Services;
using TelimAPI.Persistence.Services;

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
            return Ok(ApiResponses.Success(trainings, "Bütün təlimlər uğurla gətirildi."));
        }

        [HttpGet("get-by-{id}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var training = await _trainingService.GetByIdAsync(id);
            if (training == null) return NotFound(ApiResponses.Fail<object>("Training not found"));
            return Ok(ApiResponses.Success(training, "Training ugurla getirildi"));
        }
        [Authorize(Roles = "Trainer, Admin")]
        [HttpGet("expired")]
        public async Task<IActionResult> GetExpired()
        {
            var data = await _trainingService.GetExpiredAsync();
            return Ok(ApiResponses.Success(data, "expired trainings grt succesfully"));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("ongoingTrainings")]
        public async Task<IActionResult> GetOngoingWithUsers()
        {
            var result = await _trainingService.GetOngoingAsync();
            return Ok(ApiResponses.Success(result,"ON going trainings get succesfully"));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet("drafts")]
        public async Task<IActionResult> GetDrafts()
        {
            var result = await _trainingService.GetDraftsAsync();
            return Ok(ApiResponses.Success(result,"Drafts get successfully"));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("{id}/approve")]
        public async Task<IActionResult> Approve(Guid id)
        {
            Result result = await _trainingService.ApproveAsync(id);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Təsdiqləmə prosesi zamanı xəta baş verdi",
                    errors: result.Errors // Servisdən gələn xətaları birbaşa ötürürük
                ));
            }

            return Ok(ApiResponses.Success<object>(message: "Təlim uğurla təsdiqə göndərildi."));
        }

        [Authorize(Roles = "Trainer, Admin")]
        [HttpPost("create-session")]
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

        [HttpGet("GetSessions")]
        public async Task<IActionResult> GetTrainingSessionsByTrainingId(Guid trainingId)
        {
            Result<List<TrainingSessionGetDto>> result = await _trainingService.GetSessionsByTrainingIdAsync(trainingId);
            if (!result.Succeeded)
            {
                // Result.Errors siyahısını göndəririk
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
        [HttpPost("{sessionId}/attendance")]
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
        [HttpGet("sessions/{sessionId}/details")]
        [Authorize(Roles = "Trainer, Admin")] 
        public async Task<IActionResult> GetSessionDetails(Guid sessionId)
        {
            var result = await _trainingService.GetSessionDetailsWithParticipantsAsync(sessionId);

            if (!result.Succeeded)
            {
                // Əgər sessiya tapılmasa 404 qaytarırıq
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
        [HttpGet("{trainingId}/attendance")]
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
        [HttpGet("{trainingId}/high-attendance")]
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
        [HttpGet("{trainingId}/low-attendance")]
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

        [HttpPost("create")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Create([FromBody] TrainingCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
                return BadRequest(ApiResponses.Fail<object>("Məlumatlar tam deyil.", errors));
            }

            Result result = await _trainingService.CreateAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Təlim yaradılarkən xəta baş verdi.",
                    errors: result.Errors
                ));
            }

            return Ok(ApiResponses.Success<object>(
                message: "Təlim və iştirakçı siyahısı uğurla yaradıldı."
            ));
        }

        [HttpPut("update")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Update([FromBody] TrainingUpdateDto dto)
        {
            Result result = await _trainingService.UpdateAsync(dto);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Yeniləmə zamanı xəta baş verdi.",
                    errors: result.Errors));
            }

            return Ok(ApiResponses.Success<object>(message: "Təlim məlumatları uğurla yeniləndi."));
        }

        [HttpDelete("delete{id}")]
        [Authorize(Roles = "Trainer, Admin")]
        public async Task<IActionResult> Delete(Guid id)
        {
            var result = await _trainingService.DeleteAsync(id);

            if (!result.Succeeded)
            {
                
                return NotFound(ApiResponses.Fail<object>(
                    message: "Silmə əməliyyatı uğursuz oldu.",
                    errors: result.Errors
                ));
            }

            return Ok(ApiResponses.Success<object>(
                message: "Təlim uğurla silindi."
            ));
        }

    }
}
