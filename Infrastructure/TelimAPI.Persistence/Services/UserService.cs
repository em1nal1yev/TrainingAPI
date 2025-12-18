using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using TelimAPI.Application.Common.Results;
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

        public async Task<Result<UserTrainingsDto>> GetUserTrainingsAsync()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
                return Result<UserTrainingsDto>.Failure("İstifadəçi tapılmadı və ya sessiya bitib.");

            if (!Guid.TryParse(userIdClaim, out Guid userId))
                return Result<UserTrainingsDto>.Failure("İstifadəçi ID formatı yanlışdır.");

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return Result<UserTrainingsDto>.Failure("İstifadəçi bazada tapılmadı.");

            var allTrainings = await _trainingRepository.GetAllAsync();

            var userTrainings = allTrainings
                .Where(t => t.Participants != null && t.Participants.Any(p => p.UserId == userId) && t.Status != Domain.Enums.TrainingStatus.Draft)
                .Select(t => new TrainingDetailDto
                {
                    Title = t.Title,
                    Description = t.Description,
                    Courts = t.TrainingCourts?.Select(tc => tc.Court?.Name).Where(name => name != null).ToList() ?? new List<string>(),
                    Departments = t.TrainingDepartments?.Select(td => td.Department?.Name).Where(name => name != null).ToList() ?? new List<string>(),
                    ParticipantsCount = t.Participants?.Count ?? 0,
                    IsJoined = t.Participants.FirstOrDefault(p => p.UserId == userId)?.IsJoined ?? false
                })
                .ToList();

            var resultDto = new UserTrainingsDto
            {
                UserName = $"{user.Name} {user.Surname}".Trim(),
                Trainings = userTrainings
            };

            return Result<UserTrainingsDto>.Success(resultDto);
        }

        public async Task<Result> JoinTrainingAsync(Guid trainingId, Guid userId)
        {
            
            var training = await _trainingRepository.GetByIdAsync(trainingId);
            if (training == null)
            {
                return Result.Failure("Təlim tapılmadı.");
            }

            
            // if (DateTime.UtcNow > training.StartDate)
            //     return Result.Failure("Təlim artıq başlayıb, qoşulmaq mümkün deyil.");

            
            var existingParticipant = await _trainingRepository.GetParticipantByTrainingAndUserAsync(trainingId, userId);

            if (existingParticipant != null)
            {
                
                if (existingParticipant.IsJoined)
                {
                    return Result.Failure("Siz artıq bu təlimə qoşulmusunuz.");
                }

               
                existingParticipant.IsJoined = true;
                await _trainingRepository.UpdateParticipantAsync(existingParticipant);
            }
            else
            {
                
                var newParticipant = new TrainingParticipant
                {
                    TrainingId = trainingId,
                    UserId = userId,
                    IsJoined = true
                };

                await _trainingRepository.AddParticipantAsync(newParticipant);
            }

            
            await _trainingRepository.SaveAsync();

            return Result.Success();
        }
        public async Task<Result> SubmitTrainingFeedbackAsync(SubmitFeedbackRequest request, Guid userId)
        {
            
            var participant = await _trainingRepository.GetParticipantByTrainingAndUserAsync(request.TrainingId, userId);

            if (participant == null || !participant.IsJoined)
            {
                return Result.Failure("Rəy bildirmək üçün bu təlimin aktiv iştirakçısı olmalısınız.");
            }

            
            var existingFeedback = await _trainingRepository.GetFeedbackByParticipantIdAsync(participant.Id);

            if (existingFeedback != null)
            {
                return Result.Failure("Siz artıq bu təlim üçün rəy bildirmisiniz.");
            }

            
            var newFeedback = new TrainingFeedback
            {
                TrainingParticipantId = participant.Id,
                TrainingRating = request.TrainingRating,
                TrainerRating = request.TrainerRating,
                Comment = request.Comment,
                CreatedDate = DateTime.UtcNow 
            };

            
            await _trainingRepository.AddFeedbackAsync(newFeedback);
            await _trainingRepository.SaveAsync();

            return Result.Success();
        }


    }
}
