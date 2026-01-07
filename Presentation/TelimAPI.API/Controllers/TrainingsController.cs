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

    public class TrainingsController : BaseController
    {
        private readonly ITrainingService _trainingService;

        public TrainingsController(ITrainingService trainingService)
        {
            _trainingService = trainingService;
        }


        [HttpGet]
        public Task<IActionResult> GetAll()
        {
            return ExecuteAsync(() => _trainingService.GetAllAsync());
        }

        [HttpGet]
        public Task<IActionResult> GetById(Guid id)
        {
            return ExecuteAsync(() => _trainingService.GetByIdAsync(id));
        }


        [Authorize(Roles = "Trainer, Admin")]
        [HttpGet]
        public Task<IActionResult> GetExpired()
        {
            return ExecuteAsync(() => _trainingService.GetExpiredAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public Task<IActionResult> GetOngoingWithUsers()
        {
            return ExecuteAsync(() => _trainingService.GetOngoingAsync());
        }


        [Authorize(Roles = "Admin")]
        [HttpGet]
        public Task<IActionResult> GetDrafts()
        {
            return ExecuteAsync(() => _trainingService.GetDraftsAsync());
        }

        [Authorize(Roles = "Admin")]
        [HttpPut]
        public Task<IActionResult> Approve(Guid id)
        {
            return ExecuteAsync(() => _trainingService.ApproveAsync(id));
        }

        [Authorize(Roles = "Trainer, Admin")]
        [HttpPost]
        public Task<IActionResult> Create([FromBody] TrainingCreateDto dto)
        {
            return ExecuteAsync(() => _trainingService.CreateAsync(dto));
        }

        [Authorize(Roles = "Trainer, Admin")]
        [HttpPut]
        public Task<IActionResult> Update([FromBody] TrainingUpdateDto dto)
        {
            return ExecuteAsync(() => _trainingService.UpdateAsync(dto));
        }

        [Authorize(Roles = "Trainer, Admin")]
        [HttpDelete]
        public Task<IActionResult> Delete(Guid id)
        {
            return ExecuteAsync(() => _trainingService.DeleteAsync(id));
        }

    }
}
