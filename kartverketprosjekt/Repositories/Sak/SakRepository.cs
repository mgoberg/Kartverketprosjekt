using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        public async Task<List<SakModel>> GetUserCasesAsync(string userEmail)
        {
            return await _context.Sak
                .Where(s => s.epost_bruker == userEmail)
                .ToListAsync();
        }

        public async Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId)
        {
            return await _context.Kommentar
                .Where(c => c.SakID == sakId)
                .ToListAsync();
        }

        public async Task<bool> DeleteCaseAsync(int id)
        {
            var sak = await _context.Sak
                .Include(s => s.Kommentarer)
                .FirstOrDefaultAsync(s => s.id == id);

            if (sak != null)
            {
                _context.Kommentar.RemoveRange(sak.Kommentarer);
                _context.Sak.Remove(sak);
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<List<SelectListItem>> GetCaseworkersAsync()
        {
            return await _context.Bruker
                .Where(b => b.tilgangsnivaa_id == 3)
                .Select(b => new SelectListItem { Value = b.epost, Text = b.navn })
                .ToListAsync();
        }

        public async Task<int> RegisterCaseAsync(SakModel sak)
        {
            _context.Sak.Add(sak);
            await _context.SaveChangesAsync();
            return sak.id;
        }

        public SakModel GetCaseById(int id)
        {
            return _context.Sak.FirstOrDefault(s => s.id == id);
        }

        public async Task UpdateStatus(int id, string status)
        {
            var sak = await _context.Sak.FindAsync(id);
            if (sak != null && sak.status != status)
            {
                sak.status = status;
                sak.status_endret = true;
                await _context.SaveChangesAsync();
            }
        }

        public async Task AssignSaksbehandler(int sakId, string saksbehandlerEpost)
        {
            var sak = await _context.Sak.FindAsync(sakId);
            if (sak != null)
            {
                var saksbehandler = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == saksbehandlerEpost);
                if (saksbehandler != null)
                {
                    sak.SaksbehandlerId = saksbehandler.epost;
                    await _context.SaveChangesAsync();
                }
            }
        }

        public List<SakModel> GetAllSaker()
        {
            return _context.Sak.Include(s => s.Saksbehandler).ToList();
        }

        // New methods
        public async Task<BrukerModel> GetUserByEmailAsync(string email)
        {
            return await _context.Bruker
                .FirstOrDefaultAsync(b => b.epost == email);
        }

        public async Task<BrukerModel> GetLeastAssignedCaseworkerAsync()
        {
            return await _context.Bruker
                .Where(b => b.tilgangsnivaa_id == 3)
                .OrderBy(b => _context.Sak.Count(s => s.SaksbehandlerId == b.epost))
                .FirstOrDefaultAsync();
        }

    }
}

