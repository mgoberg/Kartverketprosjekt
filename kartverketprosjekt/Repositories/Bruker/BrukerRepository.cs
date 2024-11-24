using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;
using kartverketprosjekt.Data;

namespace kartverketprosjekt.Repositories.Bruker
{
    public class BrukerRepository : IBrukerRepository
    {
        private readonly KartverketDbContext _context;

        public BrukerRepository(KartverketDbContext context)
        {
            _context = context;
        }

        public async Task<List<BrukerModel>> GetAllUsersAsync()
        {
            return await _context.Bruker.ToListAsync();
        }

        public async Task<BrukerModel?> GetUserByIdAsync(string userId)
        {
            return await _context.Bruker.FindAsync(userId);
        }

        public async Task<BrukerModel?> GetUserByEmailAsync(string email)
        {
            return await _context.Bruker.FirstOrDefaultAsync(u => u.epost == email);
        }
        public async Task AddUserAsync(BrukerModel user)
        {
            _context.Bruker.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteUserAsync(BrukerModel user)
        {
            _context.Bruker.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<int> GetUserCountAsync()
        {
            return await _context.Bruker.CountAsync();
        }
        public async Task<List<BrukerModel>> GetUsersByAccessLevelAsync(int accessLevel)
        {
            return await _context.Bruker
                .Where(user => user.tilgangsnivaa_id == accessLevel)
                .ToListAsync();
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
