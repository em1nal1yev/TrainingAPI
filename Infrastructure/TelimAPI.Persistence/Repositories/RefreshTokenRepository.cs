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
    public class RefreshTokenRepository : IRefreshTokenRepository
    {
        private readonly AppDbContext _context;

        public RefreshTokenRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(RefreshToken token)
        {
            await _context.RefreshTokens.AddAsync(token);
            await _context.SaveChangesAsync();
        }

        public async Task<List<RefreshToken>> GetAllValidTokensForUserAsync(Guid userId)
        {
            return await _context.RefreshTokens
                .Where(t=> t.UserId == userId && !t.IsRevoked && t.Expires >= DateTime.UtcNow)
                .ToListAsync();
        }

        public async Task<RefreshToken?> GetByTokenAsync(string token)
        {
            return await _context.RefreshTokens.Include(t => t.User).FirstOrDefaultAsync(t => t.Token == token);
        }

        public async Task UpdateAsync(RefreshToken token)
        {
            _context.RefreshTokens.Update(token);
            await _context.SaveChangesAsync();
        }
    }
}
