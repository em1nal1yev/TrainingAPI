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

            if (training.Date == null)
                throw new Exception("Training date is not set.");

            var now = DateTime.UtcNow; 
            var joinStart = training.Date.Value.AddMinutes(-10); 
            var joinEnd = training.Date.Value.AddMinutes(20); 

            if (now < joinStart || now > joinEnd)
                throw new Exception("You can join the training only from 10 minutes before to 20 minutes after the start.");



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


                    await _trainingRepository.UpdateParticipantAsync(existingParticipant);
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
        public async Task SubmitTrainingFeedbackAsync(SubmitFeedbackRequest request, Guid userId)
        {
            
            var participant = await _trainingRepository.GetParticipantByTrainingAndUserAsync(request.TrainingId, userId);

            if (participant == null || !participant.IsJoined)
            {
                
                throw new Exception("You must be an active participant of this training to submit feedback.");
            }


            
            var existingFeedback = await _trainingRepository.GetFeedbackByParticipantIdAsync(participant.Id);

            if (existingFeedback != null)
            {
                throw new Exception("You have already submitted feedback for this training.");
            }

            
            var newFeedback = new TrainingFeedback
            {
                
                TrainingParticipantId = participant.Id,
                TrainingRating = request.TrainingRating,
                TrainerRating = request.TrainerRating,
                Comment = request.Comment
               
            };

            
            await _trainingRepository.AddFeedbackAsync(newFeedback);
        }
    }
}
