using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TelimAPI.Application.DTOs.Training
{
    public record TrainingCreateDto
    (
        string Title,
        string Description,
        DateTime? SelectionDeadline,
        List<Guid>? CourtIds,
        List<Guid>? DepartmentIds
    );

    public record TrainingUpdateDto(
        Guid Id,
        string Title,
        string? Description,
        DateTime? SelectionDeadline,
        List<Guid>? CourtIds,
        List<Guid>? DepartmentIds
    );

    public record TrainingGetDto(
       Guid Id,
       string? Title,
       string? Description,
       DateTime? SelectionDeadline,
       List<string>? Courts,
       List<string>? Departments
   );
}
