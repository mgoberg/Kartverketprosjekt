using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;


namespace kartverketprosjekt.Repositories.Notifikasjon
{
    public class NotifikasjonRepository : INotifikasjonRepository
    {
        private readonly KartverketDbContext _context;

        public NotifikasjonRepository(KartverketDbContext context)
        {
            _context = context;
        }

        // Check if any case has a changed status for a user
        public async Task<bool> HasStatusChangedAsync(string brukerEpost)
        {
            return await _context.Sak
                .AnyAsync(s => s.epost_bruker == brukerEpost && s.status_endret);
        }

        // Reset the notification status for a user
        public async Task<bool> ResetNotificationStatusAsync(string brukerEpost)
        {
            var saker = await _context.Sak
                .Where(s => s.epost_bruker == brukerEpost && s.status_endret)
                .ToListAsync();

            foreach (var sak in saker)
            {
                sak.status_endret = false;
            }

            await _context.SaveChangesAsync();
            return true;
        }

    }
}
