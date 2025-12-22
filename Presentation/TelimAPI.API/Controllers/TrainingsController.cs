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
    [Route("api/[controller]/[action]")]
    [ApiController]

    public class TrainingsController : ControllerBase
    {
        private readonly ITrainingService _trainingService;

        public TrainingsController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }


        [HttpGet]

        public async Task<IActionResult> GetAll()
        {
            var trainings = await _trainingService.GetAllAsync();
            return Ok(ApiResponses.Success(trainings, "Bütün təlimlər uğurla gətirildi."));
        }

        [HttpGet]
        public async Task<IActionResult> GetById(Guid id)
        {
            var training = await _trainingService.GetByIdAsync(id);
            if (training == null) return NotFound(ApiResponses.Fail<object>("Training not found"));
            return Ok(ApiResponses.Success(training, "Training ugurla getirildi"));
        }
        [Authorize(Roles = "Trainer, Admin")]
        [HttpGet]
        public async Task<IActionResult> GetExpired()
        {
            var data = await _trainingService.GetExpiredAsync();
            return Ok(ApiResponses.Success(data, "expired trainings grt succesfully"));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetOngoingWithUsers()
        {
            var result = await _trainingService.GetOngoingAsync();
            return Ok(ApiResponses.Success(result,"ON going trainings get succesfully"));
        }
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> GetDrafts()
        {
            var result = await _trainingService.GetDraftsAsync();
            return Ok(ApiResponses.Success(result,"Drafts get successfully"));
        }
        [Authorize(Roles = "Admin")]
        [HttpPut]
        public async Task<IActionResult> Approve(Guid id)
        {
            Result result = await _trainingService.ApproveAsync(id);

            if (!result.Succeeded)
            {
                return BadRequest(ApiResponses.Fail<object>(
                    message: "Təsdiqləmə prosesi zamanı xəta baş verdi",
                    errors: result.Errors 
                ));
            }

            return Ok(ApiResponses.Success<object>(message: "Təlim uğurla təsdiqə göndərildi."));
        }

        [HttpPost]
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

        [HttpPut]
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

        [HttpDelete]
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
