using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Application.Repositories;
using TelimAPI.Domain.Entities;
using TelimAPI.Persistence.Contexts;

namespace TelimAPI.Persistence.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AppDbContext _context;

        public DepartmentRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Department department)
        {
            await _context.AddAsync(department);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Guid id)
        {
            var department = await _context.Departments.FirstOrDefaultAsync(d => d.Id == id);
            if (department != null)
            {
                _context.Departments.Remove(department);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<List<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.Court)
                .Include(d => d.Users)
                .Include(d => d.TrainingDepartments)
                    .ThenInclude(d => d.Training)
                .ToListAsync();
        }

        public async Task<Department?> GetByIdAsync(Guid id)
        {
            return await _context.Departments
                .Include(d => d.Court)
                .Include(d => d.Users)
                .Include(d => d.TrainingDepartments)
                    .ThenInclude(d => d.Training)
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<List<Department>> GetByIdsAsync(IEnumerable<Guid> ids)
        {
            return await _context.Departments
                .Where(d => ids.Contains(d.Id))
                .ToListAsync();
        }

        public void Update(Department department)
        {
            _context.Update(department);
            _context.SaveChanges();
        }
    }
}
