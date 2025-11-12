using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Application.Repositories;
using TelimAPI.Application.Services;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Services
{
    public class TrainingService : ITrainingService
    {
        private readonly ITrainingRepository _trainingRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;

        public TrainingService(ITrainingRepository trainingRepository, ICourtRepository courtRepository, IDepartmentRepository departmentRepository, IUserRepository userRepository)
        {
            _trainingRepository = trainingRepository;
            _courtRepository = courtRepository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
        }

        public async Task<List<TrainingGetDto>> GetAllAsync()
        {
            var trainings = await _trainingRepository.GetAllAsync();
            return trainings.Select(t => new TrainingGetDto(
                t.Id,
                t.Title,
                t.Description,
                t.SelectionDeadline,
                t.TrainingCourts?.Select(c => c.Court.Name ?? "").ToList(),
                t.TrainingDepartments?.Select(d => d.Department.Name ?? "").ToList()
                )).ToList();
        }

        public async Task<TrainingGetDto?> GetByIdAsync(Guid id)
        {
            var training = await _trainingRepository.GetByIdAsync(id);
            if (training == null)
            {
                return null;
            }

            return new TrainingGetDto(
                training.Id,
                training.Title,
                training.Description,
                training.SelectionDeadline,
                training.TrainingCourts?.Select(c => c.Court.Name ?? "").ToList(),
                training.TrainingDepartments?.Select(d => d.Department.Name ?? "").ToList());

        }
        public async Task CreateAsync(TrainingCreateDto dto)
        {
            var training = new Training
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                SelectionDeadline = dto.SelectionDeadline,
                CreatedDate = DateTime.UtcNow,
                
            };



            
            var allCourts = await _courtRepository.GetAllAsync();

            
            List<Court> selectedCourts;

            if (dto.CourtIds == null || dto.CourtIds.Count == 0)
            {
                
                selectedCourts = allCourts;
            }
            else
            {
                
                selectedCourts = allCourts.Where(c => dto.CourtIds.Contains(c.Id)).ToList();
            }

            
            training.TrainingCourts = selectedCourts.Select(c => new TrainingCourt
            {
                CourtId = c.Id,
                TrainingId = training.Id
            }).ToList();



            
            var allDepartments = await _departmentRepository.GetAllAsync();
            List<Department> selectedDepartments;

            if (dto.DepartmentIds == null || dto.DepartmentIds.Count == 0)
            {
                selectedDepartments = allDepartments;
            }
            else
            {
                selectedDepartments = allDepartments.Where(d => dto.DepartmentIds.Contains(d.Id)).ToList();
            }

            training.TrainingDepartments = selectedDepartments.Select(d => new TrainingDepartment
            {
                DepartmentId = d.Id,
                TrainingId = training.Id
            }).ToList();



            
            var allUsers = await _userRepository.GetAllAsync();

            List<User> targetUsers = new List<User>();

            foreach (var user in allUsers)
            {
                bool courtMatch = dto.CourtIds == null || dto.CourtIds.Contains(user.CourtId);
                bool departmentMatch = dto.DepartmentIds == null || dto.DepartmentIds.Contains(user.DepartmentId);

                if (courtMatch && departmentMatch)
                {
                    targetUsers.Add(user);
                }
            }

            
            training.Participants = targetUsers.Select(u => new TrainingParticipant
            {
                TrainingId = training.Id,
                UserId = u.Id,
                IsJoined = false
            }).ToList();



            await _trainingRepository.AddAsync(training);

        }
        public async Task UpdateAsync(TrainingUpdateDto dto)
        {
            Training training = await _trainingRepository.GetByIdAsync(dto.Id);
            if (training == null)
                throw new Exception("Training not found");
            
            training.Title = dto.Title; 
            training.Description = dto.Description;
            training.SelectionDeadline = dto.SelectionDeadline;

            _trainingRepository.Update(training);
        }

        public async Task DeleteAsync(Guid id)
        {
            await _trainingRepository.DeleteAsync(id);
        }


    }
}
              