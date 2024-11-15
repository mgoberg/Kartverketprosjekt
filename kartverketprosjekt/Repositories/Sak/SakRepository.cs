using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace kartverketprosjekt.Repositories.Sak
{
    public class SakRepository : ISakRepository
    {
        private readonly KartverketDbContext _context;

        public SakRepository(KartverketDbContext context)
        {
            _context = context;
        }

        public async Task<int> GetCaseCountAsync()
        {
            return await _context.Sak.CountAsync();
        }

        public async Task<int> GetCaseCountByStatusAsync(string status)
        {
            return await _context.Sak.CountAsync(s => s.status == status);
        }

        public async Task<List<SakModel>> GetAllCasesAsync()
        {
            return await _context.Sak.ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}

