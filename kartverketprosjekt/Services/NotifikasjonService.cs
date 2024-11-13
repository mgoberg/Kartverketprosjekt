using kartverketprosjekt.Data;
using kartverketprosjekt.Services;
using Microsoft.EntityFrameworkCore;

public class NotifikasjonService : INotifikasjonService
{
    private readonly KartverketDbContext _context;

    public NotifikasjonService(KartverketDbContext context)
    {
        _context = context;
    }

    public async Task<bool> HarEndretStatus(string brukerEpost)
    {
        try
        {
            // Sjekk om det finnes minst én sak hvor brukeren har endret status
            var hasStatusChanged = await _context.Sak
                .AnyAsync(s => s.epost_bruker == brukerEpost && s.status_endret == true);

            if (hasStatusChanged)
            {
                // Logg i konsollen for debugging
                Console.WriteLine("Du har en notifikasjon.");
                return true; // Return true for notification
            }

            return false; // No notification
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Feil ved henting av sak: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> ResetNotificationStatus(string brukerEpost)
    {
        try
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
        catch (Exception ex)
        {
            Console.WriteLine($"Feil ved tilbakestilling av notifikasjoner: {ex.Message}");
            return false;
        }
    }
}