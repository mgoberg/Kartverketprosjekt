using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;

namespace kartverketprosjekt.Services
{
    public class SakService : ISakService
    {
        private readonly KartverketDbContext _context;

        public SakService(KartverketDbContext context)
        {
            _context = context;
        }
        public async Task<List<SakModel>> GetUserCasesAsync(string userEmail)
        {
            if (string.IsNullOrEmpty(userEmail))
            {
                throw new ArgumentException("User email cannot be null or empty.", nameof(userEmail));
            }

            return await _context.Sak
                .Where(s => s.epost_bruker == userEmail)
                .ToListAsync();
        }
        public async Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId)
        {
            // Retrieve comments for the given sakId
            var kommentarer = await _context.Kommentar
                .Where(c => c.SakID == sakId)
                .ToListAsync();

            return kommentarer;
        }

        public async Task<bool> DeleteCaseAsync(int id)
        {
            // Retrieve the case along with its associated comments
            var sak = await _context.Sak
                .Include(s => s.Kommentarer)
                .FirstOrDefaultAsync(s => s.id == id);

            // Check if the case exists
            if (sak != null)
            {
                // Remove associated comments
                _context.Kommentar.RemoveRange(sak.Kommentarer);

                // Remove the case
                _context.Sak.Remove(sak);
                await _context.SaveChangesAsync();

                return true; // Indicate successful deletion
            }

            return false; // Indicate failure to delete
        }
    }
}
