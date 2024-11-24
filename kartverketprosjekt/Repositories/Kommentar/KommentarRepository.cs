using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Kommentar;
using Microsoft.EntityFrameworkCore;

public class KommentarRepository : IKommentarRepository
{
    private readonly KartverketDbContext _context;

    public KommentarRepository(KartverketDbContext context)
    {
        _context = context;
    }

    public async Task AddCommentAsync(int sakID, string kommentar, string brukerEpost)
    {
        if (sakID <= 0 || string.IsNullOrWhiteSpace(kommentar))
            throw new ArgumentException("Ugyldig sakID eller kommentar.");

        var sak = await _context.Sak.FindAsync(sakID);
        if (sak == null) throw new Exception("Sak ikke funnet.");

        var nyKommentar = new KommentarModel
        {
            SakID = sakID,
            Tekst = kommentar,
            Dato = DateTime.Now,
            Epost = brukerEpost
        };

        sak.status_endret = true;
        _context.Kommentar.Add(nyKommentar);
        await _context.SaveChangesAsync();
    }

   
    public async Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId)
    {
        // Retrieve comments for the given sakId
        var kommentarer = await _context.Kommentar
            .Where(c => c.SakID == sakId)
            .ToListAsync();

        return kommentarer;
    }
}