using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Common.Results;
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
        public async Task<Result> CreateAsync(TrainingCreateDto dto)
        {
            try
            {
                
                if (dto.CourtIds?.Any() == true)
                {
                    await _idValidator.ValidateIdsAsync(dto.CourtIds, _courtRepository.GetByIdsAsync, "Məhkəmə tapılmadı.");
                }

                if (dto.DepartmentIds?.Any() == true)
                {
                    await _idValidator.ValidateIdsAsync(dto.DepartmentIds, _departmentRepository.GetByIdsAsync, "Departament tapılmadı.");
                }
            }
            catch (Exception ex)
            {
                return Result.Failure(ex.Message);
            }

            var trainingId = Guid.NewGuid();
            var training = new Training
            {
                Id = trainingId,
                Title = dto.Title,
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                CreatedDate = DateTime.UtcNow,
                Status = TrainingStatus.Draft
            };

            
            var allCourts = await _courtRepository.GetAllAsync();
            var allDepartments = await _departmentRepository.GetAllAsync();
            var allUsers = await _userRepository.GetAllAsync();

            
            var selectedCourts = (dto.CourtIds == null || !dto.CourtIds.Any())
                ? allCourts
                : allCourts.Where(c => dto.CourtIds.Contains(c.Id)).ToList();

            training.TrainingCourts = selectedCourts.Select(c => new TrainingCourt
            {
                CourtId = c.Id,
                TrainingId = trainingId
            }).ToList();

            
            var courtIdsForFiltering = selectedCourts.Select(c => c.Id).ToList();
            var departmentsOfSelectedCourts = allDepartments
                .Where(d => courtIdsForFiltering.Contains(d.CourtId))
                .ToList();

            
            var selectedDepartments = (dto.DepartmentIds == null || !dto.DepartmentIds.Any())
                ? departmentsOfSelectedCourts
                : departmentsOfSelectedCourts.Where(d => dto.DepartmentIds.Contains(d.Id)).ToList();

            training.TrainingDepartments = selectedDepartments.Select(d => new TrainingDepartment
            {
                DepartmentId = d.Id,
                TrainingId = trainingId
            }).ToList();

            
            var selectedDeptIds = selectedDepartments.Select(d => d.Id).ToHashSet();
            var selectedCourtIds = selectedCourts.Select(c => c.Id).ToHashSet();

            training.Participants = allUsers
                .Where(u => selectedCourtIds.Contains(u.CourtId) && selectedDeptIds.Contains(u.DepartmentId))
                .Select(u => new TrainingParticipant
                {
                    TrainingId = trainingId,
                    UserId = u.Id,
                    IsJoined = false
                })
                .ToList();

            
            await _trainingRepository.AddAsync(training);
            await _trainingRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result> UpdateAsync(TrainingUpdateDto dto)
        {
            
            var training = await _trainingRepository.GetByIdAsync(dto.Id);
            if (training == null)
                return Result.Failure("Təlim tapılmadı.");

            
            if (dto.Title != null) training.Title = dto.Title;
            if (dto.Description != null) training.Description = dto.Description;
            if (dto.StartDate.HasValue) training.StartDate = dto.StartDate.Value;
            if (dto.EndDate.HasValue) training.EndDate = dto.EndDate.Value;

            var allCourts = await _courtRepository.GetAllAsync();
            var allDepartments = await _departmentRepository.GetAllAsync();
            var allUsers = await _userRepository.GetAllAsync();

            // --- MƏHKƏMƏ (COURT) MƏNTİQİ ---
            List<Court> effectiveCourts;
            if (dto.CourtIds != null) // null deyilsə dəyişiklik var
            {
                effectiveCourts = dto.CourtIds.Count == 0
                    ? allCourts
                    : allCourts.Where(c => dto.CourtIds.Contains(c.Id)).ToList();

                training.TrainingCourts.Clear();
                foreach (var court in effectiveCourts)
                {
                    training.TrainingCourts.Add(new TrainingCourt { CourtId = court.Id, TrainingId = training.Id });
                }
            }
            else // null-dırsa mövcud olanlar qalır
            {
                var currentCourtIds = training.TrainingCourts.Select(tc => tc.CourtId).ToList();
                effectiveCourts = allCourts.Where(c => currentCourtIds.Contains(c.Id)).ToList();
            }

            // --- DEPARTAMENT MƏNTİQİ ---
            List<Department> selectedDepartments;
            var courtIdsForFiltering = effectiveCourts.Select(c => c.Id).ToList();
            var filteredDepartmentsByCourt = allDepartments.Where(d => courtIdsForFiltering.Contains(d.CourtId)).ToList();

            if (dto.DepartmentIds != null) // null deyilsə dəyişiklik var
            {
                selectedDepartments = dto.DepartmentIds.Count == 0
                    ? filteredDepartmentsByCourt
                    : filteredDepartmentsByCourt.Where(d => dto.DepartmentIds.Contains(d.Id)).ToList();

                training.TrainingDepartments.Clear();
                foreach (var dep in selectedDepartments)
                {
                    training.TrainingDepartments.Add(new TrainingDepartment { DepartmentId = dep.Id, TrainingId = training.Id });
                }
            }
            else // DepartamentIds null-dırsa
            {
                if (dto.CourtIds != null) 
                {
                    selectedDepartments = filteredDepartmentsByCourt;
                    training.TrainingDepartments.Clear();
                    foreach (var dep in selectedDepartments)
                    {
                        training.TrainingDepartments.Add(new TrainingDepartment { DepartmentId = dep.Id, TrainingId = training.Id });
                    }
                }
                else 
                {
                    var currentDepIds = training.TrainingDepartments.Select(td => td.DepartmentId).ToList();
                    selectedDepartments = allDepartments.Where(d => currentDepIds.Contains(d.Id)).ToList();
                }
            }

            
            if (dto.CourtIds != null || dto.DepartmentIds != null)
            {
                var finalCourtIds = effectiveCourts.Select(c => c.Id).ToHashSet();
                var finalDeptIds = selectedDepartments.Select(d => d.Id).ToHashSet();

                var targetUsers = allUsers.Where(u =>
                    finalCourtIds.Contains(u.CourtId) && finalDeptIds.Contains(u.DepartmentId)).ToList();

                training.Participants.Clear();
                foreach (var user in targetUsers)
                {
                    training.Participants.Add(new TrainingParticipant { TrainingId = training.Id, UserId = user.Id, IsJoined = false });
                }
            }

            _trainingRepository.Update(training);
            await _trainingRepository.SaveAsync();

            return Result.Success();
        }

        public async Task<Result> DeleteAsync(Guid id)
        {
            var training = await _trainingRepository.GetByIdAsync(id);

            if (training == null)
            {
                return Result.Failure("Silinmək istənən təlim tapılmadı.");
            }
            await _trainingRepository.DeleteAsync(id);
            await _trainingRepository.SaveAsync(); 

            return Result.Success();
        }

        public async Task<Result<List<TrainingSessionGetDto>>> GetSessionsByTrainingIdAsync(Guid trainingId)
        {
            var training = await _trainingRepository.GetByIdAsync(trainingId);
            if (training == null)
            {
                return Result<List<TrainingSessionGetDto>>.Failure("Təlim tapılmadı (Training not found).");
            }

            var joinedParticipants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            var totalJoinedParticipants = joinedParticipants.Count;

            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);

            var dtos = sessions.Select(s => new TrainingSessionGetDto(
                s.Id,
                s.StartTime,
                s.EndTime,
                totalJoinedParticipants,
                s.Attendances?.Count(a => a.IsPresent) ?? 0
            )).ToList();

            return Result<List<TrainingSessionGetDto>>.Success(dtos);
        }

        public async Task<Result<TrainingSessionGetDto>> CreateSessionAsync(TrainingSessionCreateDto sessionDto)
        {
            var training = await _trainingRepository.GetByIdAsync(sessionDto.TrainingId);

            if (training == null)
                return Result<TrainingSessionGetDto>.Failure("Təlim tapılmadı.");

            if (sessionDto.StartTime < training.StartDate || sessionDto.EndTime > training.EndDate)
                return Result<TrainingSessionGetDto>.Failure("Sessiya vaxtı təlimin tarix aralığında olmalıdır.");

            var sessionEntity = new TrainingSession
            {
                TrainingId = sessionDto.TrainingId,
                Title = sessionDto.Title,
                StartTime = sessionDto.StartTime,
                EndTime = sessionDto.EndTime
                
            };

            
            await _trainingRepository.AddTrainingSessionAsync(sessionEntity);

            var resultDto = new TrainingSessionGetDto(
                sessionEntity.Id,
                sessionEntity.StartTime,
                sessionEntity.EndTime,
                0,
                0
            );

            return Result<TrainingSessionGetDto>.Success(resultDto);

        }

        public async Task<Result> AddSessionAttendanceAsync(Guid sessionId, List<SessionAttendanceDto> attendanceDtos)
        {
            var session = await _trainingRepository.GetSessionByIdAsync(sessionId);
            if (session == null)
            {
                return Result.Failure($"Training session with ID {sessionId} not found.");
            }

            var deadline = session.EndTime.AddDays(1);
            if (DateTime.UtcNow > deadline)
            {
                return Result.Failure("Davamiyyət qeydi üçün vaxt bitib (Deadline expired).");
            }

            var attendancesToInsert = new List<SessionAttendance>();
            var attendancesToUpdate = new List<SessionAttendance>();

            var trainingId = session.TrainingId;
            var joinedParticipants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);

            var joinedUserIds = joinedParticipants.Select(p => p.UserId).ToHashSet();

            var incomingUserIds = attendanceDtos.Select(dto => dto.UserId).Distinct().ToList();
            var existingUserIds = await _userRepository.GetExistingUserIdsAsync(incomingUserIds);

            var validationErrors = new List<string>();
            foreach (var dto in attendanceDtos)
            {

                if (!existingUserIds.Contains(dto.UserId))
                {
                    validationErrors.Add($"İstifadəçi ID {dto.UserId} sistemdə tapılmadı.");
                    continue;
                }

                if (!joinedUserIds.Contains(dto.UserId))
                {
                    validationErrors.Add($"İstifadəçi ID {dto.UserId} bu təlimin iştirakçısı deyil.");
                    continue;
                }



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

            if (validationErrors.Any())
            {
                return Result.Failure(validationErrors);
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
            }

            if (attendancesToInsert.Any() || attendancesToUpdate.Any())
            {
                await _trainingRepository.SaveChangesAsync();
            }

            return Result.Success();

        }



        public async Task<Result<SessionDetailsDto>> GetSessionDetailsWithParticipantsAsync(Guid sessionId)
        {
            
            var session = await _trainingRepository.GetSessionByIdAsync(sessionId);

            if (session == null)
            {
                return Result<SessionDetailsDto>.Failure($"ID-si {sessionId} olan təlim sessiyası tapılmadı.");
            }

            var attendances = await _trainingRepository.GetAllAttendancesBySessionIdAsync(sessionId);
            var attendanceMap = attendances.ToDictionary(a => a.UserId, a => a.IsPresent);

            var participantsDto = session.Training.Participants
                .Where(p => p.IsJoined) 
                .Select(p => new SessionParticipantDto
                {
                    UserId = p.UserId,
                    UserName = $"{p.User.Name} {p.User.Surname}",
                    IsJoined = p.IsJoined,
                    IsPresent = attendanceMap.ContainsKey(p.UserId) ? attendanceMap[p.UserId] : (bool?)null
                })
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

            return Result<SessionDetailsDto>.Success(sessionDetailsDto);
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

        public async Task<Result> ApproveAsync(Guid id)
        {
            var training = await _trainingRepository.GetByIdAsync(id);

            if (training == null)
            {
                return new Result
                {
                    Succeeded = false,
                    Errors = new List<string> { "Təlim tapılmadı" }
                };
            }
                

            if (training.Status != TrainingStatus.Draft)
            {
                return new Result
                {
                    Succeeded = false,
                    Errors = new List<string> { "Təlim 'Draft' statusunda deyil, təsdiqlənə bilməz" }
                };
            }
                

            training.Status = TrainingStatus.Pending;

            await _trainingRepository.SaveAsync();
            return new Result { Succeeded = true };
        }

        public async Task<Result<TrainingAttendanceSummaryDto>> GetTrainingAttendancesAsync(Guid trainingId)
        {
            var training = await _trainingRepository.GetByIdAsync(trainingId);
            if (training == null)
            {
                return Result<TrainingAttendanceSummaryDto>.Failure("Təlim tapılmadı.");
            }

            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);
            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);

            var result = new TrainingAttendanceSummaryDto
            {
                TrainingId = training.Id,
                TrainingTitle = training.Title,
                Sessions = new List<SessionAttendanceResultDto>() 
            };

            foreach (var session in sessions)
            {
                var sessionResult = new SessionAttendanceResultDto
                {
                    SessionId = session.Id,
                    StartDate = session.StartTime,
                    EndDate = session.EndTime,
                    Attendances = new List<UserAttendanceDto>()
                };

                foreach (var participant in participants)
                {
                    // Bu sessiya üçün həmin istifadəçinin davamiyyəti varmı?
                    var attendance = session.Attendances?
                        .FirstOrDefault(a => a.UserId == participant.UserId);

                    sessionResult.Attendances.Add(new UserAttendanceDto
                    {
                        UserId = participant.UserId,
                        UserName = $"{participant.User.Name} {participant.User.Surname}",
                        IsPresent = attendance?.IsPresent ?? false
                    });
                }

                result.Sessions.Add(sessionResult);
            }

            return Result<TrainingAttendanceSummaryDto>.Success(result);
        }

        public async Task<Result<List<HighAttendanceDto>>> GetHighAttendanceAsync(Guid trainingId)
        {

            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            if (!participants.Any())
                return Result<List<HighAttendanceDto>>.Success(new List<HighAttendanceDto>());


            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);
            int totalSessions = sessions.Count();
            if (totalSessions == 0)
                return Result<List<HighAttendanceDto>>.Success(new List<HighAttendanceDto>());

            var allAttendances = sessions
                .SelectMany(s => s.Attendances ?? new List<SessionAttendance>())
                .ToList();

            var result = new List<HighAttendanceDto>();

            foreach (var participant in participants)
            {
                int attendedCount = allAttendances.Count(a =>
                    a.UserId == participant.UserId && a.IsPresent);

                double rate = (double)attendedCount / totalSessions * 100;

                if (rate > 75)
                {
                    result.Add(new HighAttendanceDto
                    {
                        UserId = participant.UserId,
                        Fullname = $"{participant.User?.Name} {participant.User?.Surname}".Trim(),
                        TotalSessions = totalSessions,
                        AttendedSessions = attendedCount,
                        AttendanceRate = Math.Round(rate, 2)
                    });
                }
            }
            return Result<List<HighAttendanceDto>>.Success(result);

        }

        public async Task<Result<List<HighAttendanceDto>>> GetLowAttendanceAsync(Guid trainingId)
        {

            var participants = await _trainingRepository.GetJoinedParticipantsByTrainingIdAsync(trainingId);
            var sessions = await _trainingRepository.GetSessionsByTrainingIdAsync(trainingId);

            if (!participants.Any() || !sessions.Any())
            {
                return Result<List<HighAttendanceDto>>.Success(new List<HighAttendanceDto>());
            }

            int totalSessions = sessions.Count();

            var allAttendances = sessions
                .SelectMany(s => s.Attendances ?? new List<SessionAttendance>())
                .ToList();

            var result = new List<HighAttendanceDto>();

            foreach (var participant in participants)
            {
                int attendedCount = allAttendances.Count(a =>
                    a.UserId == participant.UserId && a.IsPresent);

                double rate = (double)attendedCount / totalSessions * 100;

                
                if (rate < 75)
                {
                    result.Add(new HighAttendanceDto
                    {
                        UserId = participant.UserId,
                        Fullname = $"{participant.User?.Name} {participant.User?.Surname}".Trim(),
                        TotalSessions = totalSessions,
                        AttendedSessions = attendedCount,
                        AttendanceRate = Math.Round(rate, 2)
                    });
                }
            }

            return Result<List<HighAttendanceDto>>.Success(result);
        }

    }
}
              