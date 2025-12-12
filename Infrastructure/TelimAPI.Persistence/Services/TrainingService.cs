using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.DTOs.Training;
using TelimAPI.Application.Helper;
using TelimAPI.Application.Repositories;
using TelimAPI.Application.Services;
using TelimAPI.Domain.Entities;
using TelimAPI.Domain.Enums;

namespace TelimAPI.Persistence.Services
{
    public class TrainingService : ITrainingService
    {
        IIdValidationService _idValidator;
        private readonly ITrainingRepository _trainingRepository;
        private readonly ICourtRepository _courtRepository;
        private readonly IDepartmentRepository _departmentRepository;
        private readonly IUserRepository _userRepository;

        public TrainingService(ITrainingRepository trainingRepository, ICourtRepository courtRepository, IDepartmentRepository departmentRepository, IUserRepository userRepository, IIdValidationService idValidator)
        {
            _trainingRepository = trainingRepository;
            _courtRepository = courtRepository;
            _departmentRepository = departmentRepository;
            _userRepository = userRepository;
            _idValidator = idValidator;
        }

        public async Task<List<TrainingGetDto>> GetAllAsync()
        {
            var trainings = await _trainingRepository.GetAllAsync();
            var filtered = trainings.Where(x => x.Status != TrainingStatus.Draft);

            return filtered.Select(t => new TrainingGetDto(
                t.Id,
                t.Title,
                t.Description,
                t.StartDate,
                t.EndDate,
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
                training.StartDate,
                training.EndDate,
                training.TrainingCourts?.Select(c => c.Court.Name ?? "").ToList(),
                training.TrainingDepartments?.Select(d => d.Department.Name ?? "").ToList()
            );

        }
        public async Task CreateAsync(TrainingCreateDto dto)
        {

            await _idValidator.ValidateIdsAsync(
                dto.CourtIds,
                _courtRepository.GetByIdsAsync,
                "One or more courts not found");

            await _idValidator.ValidateIdsAsync(
                dto.DepartmentIds,
                _departmentRepository.GetByIdsAsync,
                "One or more departments not found");


            var training = new Training
            {
                Id = Guid.NewGuid(),
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
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
            if (dto.StartDate.HasValue) training.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) training.EndDate = dto.EndDate.Value;


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
            var joinedParticipants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            var totalJoinedParticipants = joinedParticipants.Count;

            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);

            return sessions.Select(s => new TrainingSessionGetDto(
                s.Id,
                s.StartTime, 
                s.EndTime,
                totalJoinedParticipants,
                s.Attendances?.Count(a => a.IsPresent) ?? 0
            )).ToList();
        }

        public async Task<TrainingSessionGetDto> CreateSessionAsync(TrainingSessionCreateDto sessionDto)
        {
            var training = await _trainingRepository.GetByIdAsync(sessionDto.TrainingId);

            if (training == null)
                throw new Exception("Training not found.");

            if (sessionDto.StartTime < training.StartDate || sessionDto.EndTime > training.EndDate)
                throw new Exception("Session time must be inside training date range");

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

        public async Task AddSessionAttendanceAsync(Guid sessionId, List<SessionAttendanceDto> attendanceDtos)
        {

            var attendancesToInsert = new List<SessionAttendance>();
            var attendancesToUpdate = new List<SessionAttendance>();

            var session = await _trainingRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
            {
                throw new Exception($"Training session with ID {sessionId} not found.");
            }
            var deadline = session.EndTime.AddDays(1);
            if (DateTime.UtcNow > deadline)
                throw new Exception("Attendance submission time is expired.");
            var trainingId = session.TrainingId;

            var joinedParticipants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            var joinedUserIds = joinedParticipants.Select(p => p.UserId).ToHashSet();

            var incomingUserIds = attendanceDtos.Select(dto => dto.UserId).Distinct();

            var existingUserIds = await _userRepository.GetExistingUserIdsAsync(incomingUserIds);

            foreach (var dto in attendanceDtos)
            {
                
                if (!existingUserIds.Contains(dto.UserId))
                {
                    
                    throw new Exception($"User with ID {dto.UserId} not found in the database.");
                }

                
                if (joinedUserIds.Contains(dto.UserId))
                {
                    
                    var existingAttendance = await _trainingRepository.GetAttendanceBySessionAndUserAsync(sessionId, dto.UserId);
                    

                    if (existingAttendance != null)
                    {
                        
                        if (existingAttendance.IsPresent != dto.IsPresent)
                        {
                            existingAttendance.IsPresent = dto.IsPresent;
                            existingAttendance.JoinedTime = dto.IsPresent ? (DateTime?)DateTime.UtcNow : null;
                            attendancesToUpdate.Add(existingAttendance);
                        }
                    }
                    else
                    {
                        attendancesToInsert.Add(new SessionAttendance
                        {
                            TrainingSessionId = sessionId,
                            UserId = dto.UserId,
                            IsPresent = dto.IsPresent,
                            JoinedTime = dto.IsPresent ? (DateTime?)DateTime.UtcNow : null,
                            CreatedDate = DateTime.UtcNow
                        });
                    }
                }
                else
                {
                    
                    throw new Exception($"User with ID {dto.UserId} is not a registered (IsJoined=true) participant for this training.");
                }
                if (attendancesToInsert.Any())
                {
                    await _trainingRepository.AddRangeSessionAttendanceAsync(attendancesToInsert);
                }

                if (attendancesToUpdate.Any())
                {
                    foreach (var attendance in attendancesToUpdate)
                    {
                        _trainingRepository.UpdateSessionAttendance(attendance);
                    }
                    await _trainingRepository.SaveChangesAsync();
                }
            }


        }

        public async Task<List<Guid>> GetJoinedParticipantIdsAsync(Guid trainingId)
        {
            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            return participants.Select(p => p.UserId).ToList();
        }

        public async Task<List<SessionParticipantDto>> GetSessionAttendanceListAsync(Guid sessionId)
        {
            var session = await _trainingRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
            {
                throw new Exception($"Training session with ID {sessionId} not found.");
            }

            var trainingId = session.TrainingId;

            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);

            
            
           
            var allAttendances = await _trainingRepository.GetAllAttendancesBySessionIdAsync(sessionId); 

            var attendanceMap = allAttendances.ToDictionary(a => a.UserId, a => a.IsPresent);

            
            return participants.Select(p => new SessionParticipantDto
            {
                UserId = p.UserId,
                UserName = $"{p.User.Name} {p.User.Surname}", // Ad və soyadınızı User modelinə görə birləşdirin
                IsJoined = p.IsJoined,
                IsPresent = attendanceMap.ContainsKey(p.UserId) ? attendanceMap[p.UserId] : (bool?)null
            }).ToList();
        }

        public async Task<SessionDetailsDto> GetSessionDetailsWithParticipantsAsync(Guid sessionId)
        {
            
            var session = await _trainingRepository.GetSessionByIdAsync(sessionId);

            if (session == null)
            {
                throw new Exception($"Training session with ID {sessionId} not found.");
            }

            var attendances = await _trainingRepository.GetAllAttendancesBySessionIdAsync(sessionId);
            var attendanceMap = attendances.ToDictionary(a => a.UserId, a => a.IsPresent);
            var participantsDto = session.Training.Participants
                .Select(p => new SessionParticipantDto
                {
                    UserId = p.UserId,
                    
                    UserName = $"{p.User.Name} {p.User.Surname}",
                    IsJoined = p.IsJoined,
                    IsPresent = attendanceMap.ContainsKey(p.UserId) ? attendanceMap[p.UserId] : (bool?)null

                })
                
                .Where(p => p.IsJoined == true)
                .ToList();

            
            var sessionDetailsDto = new SessionDetailsDto
            {
                SessionId = session.Id,
                Title = session.Title,
                StartDate = session.StartTime,
                EndDate = session.EndTime,
                TrainingName = session.Training.Title,
                JoinedParticipants = participantsDto
            };

            return sessionDetailsDto;
        }

        public async Task<List<TrainingGetDto>> GetExpiredAsync()
        {
            var trainings = await _trainingRepository.GetExpiredAsync();

            return trainings.Select(t => new TrainingGetDto(
                t.Id,
                t.Title,
                t.Description,
                t.StartDate,
                t.EndDate,
                t.TrainingCourts?.Select(c => c.Court.Name ?? "").ToList(),
                t.TrainingDepartments?.Select(d => d.Department.Name ?? "").ToList()
            )).ToList();
        }

        public async Task<List<TrainingGetDto>> GetDraftsAsync()
        {
            var trainings = await _trainingRepository.GetDraftsAsync();

            return trainings.Select(t => new TrainingGetDto(
                t.Id,
                t.Title,
                t.Description,
                t.StartDate,
                t.EndDate,
                t.TrainingCourts?.Select(c => c.Court.Name ?? "").ToList(),
                t.TrainingDepartments?.Select(d => d.Department.Name ?? "").ToList()
            )).ToList();
        }

        public async Task<List<TrainingOngoingWithUsersDto>> GetOngoingAsync()
        {
            var trainings = await _trainingRepository.GetOngoingAsync();

            return trainings.Select(t => new TrainingOngoingWithUsersDto
            {
                Id = t.Id,
                Title = t.Title,
                Users = t.Participants
        .Where(p => p.IsJoined)
        .Select(p => new UserSimpleDto
        {
            Id = p.UserId,
            Name = p.User.Name
        }).ToList()
            }).ToList();
        }

        public async Task<bool> ApproveAsync(Guid id)
        {
            var training = await _trainingRepository.GetByIdAsync(id);

            if (training == null)
                throw new Exception("Training not found");

            if (training.Status != TrainingStatus.Draft)
                throw new Exception("Training is not in draft state");

            training.Status = TrainingStatus.Pending;

            await _trainingRepository.SaveAsync();
            return true;
        }

        public async Task<List<HighAttendanceDto>> GetHighAttendanceAsync(Guid trainingId)
        {
            
            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            if (!participants.Any()) return new List<HighAttendanceDto>();

            var participantUserIds = participants.Select(p => p.UserId).ToList();

            
            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);
            int totalSessions = sessions.Count();
            if (totalSessions == 0) return new List<HighAttendanceDto>();

            
            var allAttendances = new List<SessionAttendance>();
            foreach (var session in sessions)
            {
                var attendances = await _trainingRepository.GetAllAttendancesBySessionIdAsync(session.Id);
                allAttendances.AddRange(attendances);
            }

            
            var result = new List<HighAttendanceDto>();

            foreach (var participant in participants)
            {
                int attended = allAttendances.Count(a =>
                    a.UserId == participant.UserId &&
                    a.IsPresent == true
                );

                double rate = (double)attended / totalSessions * 100;

                if (rate > 75) 
                {
                    result.Add(new HighAttendanceDto
                    {
                        UserId = participant.UserId,
                        Fullname = participant.User?.Name ?? "", // səndə fullname yoxdur demişdin, boş gələcək
                        TotalSessions = totalSessions,
                        AttendedSessions = attended,
                        AttendanceRate = Math.Round(rate, 2)
                    });
                }
            }

            return result;

        }

        public async Task<List<HighAttendanceDto>> GetLowAttendanceAsync(Guid trainingId)
        {

            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            if (!participants.Any()) return new List<HighAttendanceDto>();

            var participantUserIds = participants.Select(p => p.UserId).ToList();


            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);
            int totalSessions = sessions.Count();
            if (totalSessions == 0) return new List<HighAttendanceDto>();


            var allAttendances = new List<SessionAttendance>();
            foreach (var session in sessions)
            {
                var attendances = await _trainingRepository.GetAllAttendancesBySessionIdAsync(session.Id);
                allAttendances.AddRange(attendances);
            }


            var result = new List<HighAttendanceDto>();

            foreach (var participant in participants)
            {
                int attended = allAttendances.Count(a =>
                    a.UserId == participant.UserId &&
                    a.IsPresent == true
                );

                double rate = (double)attended / totalSessions * 100;

                if (rate < 75)
                {
                    result.Add(new HighAttendanceDto
                    {
                        UserId = participant.UserId,
                        Fullname = participant.User?.Name ?? "", 
                        TotalSessions = totalSessions,
                        AttendedSessions = attended,
                        AttendanceRate = Math.Round(rate, 2)
                    });
                }
            }
            if (result.Count == 0)
                throw new Exception("There are no students with low attendance.");
            
            return result;

        }

        public async Task<TrainingAttendanceSummaryDto> GetTrainingAttendancesAsync(Guid trainingId)
        {
            var training = await _trainingRepository.GetByIdAsync(trainingId);

            if (training == null)
                throw new Exception("Training not found.");

            // Sessiyaları çəkirik
            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);

            // Qoşulmuş participantları çək
            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);

            var result = new TrainingAttendanceSummaryDto
            {
                TrainingId = training.Id,
                TrainingTitle = training.Title
            };

            foreach (var session in sessions)
            {
                var sessionResult = new SessionAttendanceResultDto
                {
                    SessionId = session.Id,
                    StartDate = session.StartTime,
                    EndDate = session.EndTime
                };

                foreach (var participant in participants)
                {
                    var attendance = session.Attendances
                        .FirstOrDefault(a => a.UserId == participant.UserId);

                    sessionResult.Attendances.Add(new UserAttendanceDto
                    {
                        UserId = participant.UserId,
                        UserName = participant.User.Name,
                        IsPresent = attendance?.IsPresent ?? false
                    });
                }

                result.Sessions.Add(sessionResult);
            }

            return result;
        }
    }
}
              