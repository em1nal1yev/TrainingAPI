using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Repositories
{
    public interface ICourtRepository
    {
        Task<List<Court>> GetAllAsync();

        Task<Court?> GetByIdAsync(Guid id);
        Task<List<Court>> GetByIdsAsync(IEnumerable<Guid> ids);
    }
}
