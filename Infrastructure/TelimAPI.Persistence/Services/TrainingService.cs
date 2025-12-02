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
                t.Date,
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
                training.Date,
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
                Date = dto.Date,
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

            IEnumerable<Department> filteredDepartmentsByCourt = allDepartments;
            
            if (dto.CourtIds != null && dto.CourtIds.Count > 0)
            {
                
                filteredDepartmentsByCourt = allDepartments.Where(d => dto.CourtIds.Contains(d.CourtId));
            }

            List<Department> selectedDepartments;

            if (dto.DepartmentIds == null || dto.DepartmentIds.Count == 0)
            {
                
                selectedDepartments = filteredDepartmentsByCourt.ToList();
            }
            else
            {
                
                selectedDepartments = filteredDepartmentsByCourt.Where(d => dto.DepartmentIds.Contains(d.Id)).ToList();
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
                bool courtMatch = dto.CourtIds == null || dto.CourtIds.Count == 0 || dto.CourtIds.Contains(user.CourtId);
                bool departmentMatch = dto.DepartmentIds == null || dto.DepartmentIds.Count == 0 || dto.DepartmentIds.Contains(user.DepartmentId);

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

       

        // court ve department null gelerse deyisilmir count 0olarsa hamsi olur
        public async Task UpdateAsync(TrainingUpdateDto dto)
        {
            
            Training training = await _trainingRepository.GetByIdAsync(dto.Id);
            if (training == null)
                throw new Exception("Training not found");

            
            if (dto.Title != null) training.Title = dto.Title;
            if (dto.Description != null) training.Description = dto.Description;
            if (dto.Date.HasValue) training.Date = dto.Date.Value;

            
            var allCourts = await _courtRepository.GetAllAsync();
            var allDepartments = await _departmentRepository.GetAllAsync();
            var allUsers = await _userRepository.GetAllAsync();

            
            List<Court> effectiveCourts;

            
            if (dto.CourtIds != null)
            {
                if (dto.CourtIds.Count == 0)
                {
                    
                    effectiveCourts = allCourts;
                }
                else
                {
                    
                    effectiveCourts = allCourts.Where(c => dto.CourtIds.Contains(c.Id)).ToList();
                }

                
                training.TrainingCourts.Clear();
                training.TrainingCourts = effectiveCourts.Select(c => new TrainingCourt
                {
                    CourtId = c.Id,
                    TrainingId = training.Id
                }).ToList();
            }
            else 
            {
                var currentCourtIds = training.TrainingCourts.Select(tc => tc.CourtId).ToList();
                effectiveCourts = allCourts.Where(c => currentCourtIds.Contains(c.Id)).ToList();
            }

            
            var courtIdsForFiltering = effectiveCourts.Select(c => c.Id).ToList();

            
            IEnumerable<Department> filteredDepartmentsByCourt = allDepartments
                .Where(d => courtIdsForFiltering.Contains(d.CourtId));

            List<Department> selectedDepartments;

            
            if (dto.DepartmentIds != null)
            {
                if (dto.DepartmentIds.Count == 0)
                {
                    
                    selectedDepartments = filteredDepartmentsByCourt.ToList();
                }
                else
                {
                    
                    selectedDepartments = filteredDepartmentsByCourt
                        .Where(d => dto.DepartmentIds.Contains(d.Id)).ToList();
                }

                
                training.TrainingDepartments.Clear();
                training.TrainingDepartments = selectedDepartments.Select(d => new TrainingDepartment
                {
                    DepartmentId = d.Id,
                    TrainingId = training.Id
                }).ToList();
            }
            else 
            {
                
                if (dto.CourtIds != null)
                {
                    
                    selectedDepartments = filteredDepartmentsByCourt.ToList();

                    
                    training.TrainingDepartments.Clear();
                    training.TrainingDepartments = selectedDepartments.Select(d => new TrainingDepartment
                    {
                        DepartmentId = d.Id,
                        TrainingId = training.Id
                    }).ToList();
                }
                else
                {
                    
                    var currentDepartmentIds = training.TrainingDepartments.Select(td => td.DepartmentId).ToList();
                    selectedDepartments = allDepartments.Where(d => currentDepartmentIds.Contains(d.Id)).ToList();
                }
            }

           
            var finalCourtIds = effectiveCourts.Select(c => c.Id).ToList();
            var finalDepartmentIds = selectedDepartments.Select(d => d.Id).ToList();

            List<User> targetUsers = new List<User>();
            foreach (var user in allUsers)
            {
                bool courtMatch = finalCourtIds.Contains(user.CourtId);
                bool departmentMatch = finalDepartmentIds.Contains(user.DepartmentId);

                if (courtMatch && departmentMatch)
                {
                    targetUsers.Add(user);
                }
            }

            
            training.Participants.Clear();
            training.Participants = targetUsers.Select(u => new TrainingParticipant
            {
                TrainingId = training.Id,
                UserId = u.Id,
                IsJoined = false
            }).ToList();

            
            _trainingRepository.Update(training);
        }



        public async Task DeleteAsync(Guid id)
        {
            await _trainingRepository.DeleteAsync(id);
        }

        public async Task<List<TrainingSessionGetDto>> GetSessionsByTrainingIdAsync(Guid trainingId)
        {
            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);

            return sessions.Select(s => new TrainingSessionGetDto(
                s.Id,
                s.StartTime, 
                s.EndTime,
                s.Attendances?.Count ?? 0,
                s.Attendances?.Count(a => a.IsPresent) ?? 0
            )).ToList();
        }

        public async Task<TrainingSessionGetDto> CreateSessionAsync(TrainingSessionCreateDto sessionDto)
        {
            
            var sessionEntity = new TrainingSession
            {
                TrainingId = sessionDto.TrainingId,
                Title = sessionDto.Title,
                StartTime = sessionDto.StartTime,
                EndTime = sessionDto.EndTime
                
            };

            
            await _trainingRepository.AddTrainingSessionAsync(sessionEntity);

            
            return new TrainingSessionGetDto(
                sessionEntity.Id,
                sessionEntity.StartTime,
                sessionEntity.EndTime,
                0, 
                0
            );
        }
    }
}
              