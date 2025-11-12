using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.User;
using TelimAPI.Application.Repositories;
using TelimAPI.Application.Services;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Services
{
    public class UserService : IUserService
    {

        private readonly ITrainingRepository _trainingRepository;
        private readonly IUserRepository _userRepository;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public UserService(ITrainingRepository trainingRepository, IUserRepository userRepository, IHttpContextAccessor httpContextAccessor)
        {
            _trainingRepository = trainingRepository;
            _userRepository = userRepository;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<UserTrainingsDto> GetUserTrainingsAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (userIdClaim == null)
                throw new Exception("User is not authenticated");

            Guid userId = Guid.Parse(userIdClaim);

            
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new Exception("User not found");

            
            var trainings = await _trainingRepository.GetAllAsync();

            
            var userTrainings = trainings
                .Where(t => t.Participants.Any(p => p.UserId == userId))
                .Select(t => new TrainingDetailDto
                {
                    Title = t.Title,
                    Description = t.Description,
                    Courts = t.TrainingCourts.Select(tc => tc.Court.Name).ToList(),
                    Departments = t.TrainingDepartments.Select(td => td.Department.Name).ToList(),
                    ParticipantsCount = t.Participants.Count,
                    IsJoined = t.Participants.FirstOrDefault(p => p.UserId == userId)?.IsJoined ?? false
                })
                .ToList();

            
            return new UserTrainingsDto
            {
                UserName = user.Name,
                Trainings = userTrainings
            };
        }

        public async Task JoinTrainingAsync(Guid trainingId, Guid userId)
        {
            
            var training = await _trainingRepository.GetByIdAsync(trainingId);
            if (training == null)
                throw new Exception("Training not found.");

            
            var existingParticipant = await _trainingRepository.GetParticipantByTrainingAndUserAsync(trainingId, userId);

            if (existingParticipant != null)
            {
               
                if (existingParticipant.IsJoined)
                {
                    throw new Exception("You have already joined this training.");
                }
                else
                {

                    existingParticipant.IsJoined = true;

                    
                    _trainingRepository.UpdateParticipantAsync(existingParticipant);
                    return;
                }
            }

            
            var newParticipant = new TrainingParticipant
            {

                TrainingId = trainingId,
                UserId = userId,
                IsJoined = true
                
            };

            
            await _trainingRepository.AddParticipantAsync(newParticipant);
        }
    }
}
