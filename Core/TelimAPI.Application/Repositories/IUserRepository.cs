using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Application.Repositories
{
    public interface IUserRepository
    {
        Task<List<User>> GetAllAsync();

        Task<User?> GetByIdAsync(Guid id);

        Task AddAsync(User user);

        void Update(User user);

        Task DeleteAsync(Guid id);
    }
}
