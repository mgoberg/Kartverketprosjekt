namespace kartverketprosjekt.Services.Kommentar
{
    using kartverketprosjekt.Data;
    using kartverketprosjekt.Models;

    public class KommentarService : IKommentarService
    {
        private readonly KartverketDbContext _context;

        public KommentarService(KartverketDbContext context)
        {
            _context = context;
        }

        public async Task AddComment(int sakID, string kommentar, string brukerEpost)
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

        public List<KommentarModel> GetComments(int sakId)
        {
            return _context.Kommentar.Where(k => k.SakID == sakId).ToList();
        }
    }
}
