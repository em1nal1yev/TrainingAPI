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
