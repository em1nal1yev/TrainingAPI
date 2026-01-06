using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Repositories
{
    public interface IDepartmentRepository
    {
        Task<List<Department>> GetAllAsync();

        Task<Department?> GetByIdAsync(Guid id);
        Task<List<Department>> GetByIdsAsync(IEnumerable<Guid> ids);

        Task AddAsync(Department department);
        void Update(Department department);
        Task DeleteAsync(Guid id);
    }
}
