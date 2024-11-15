using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using kartverketprosjekt.Services.API;
using kartverketprosjekt.Services.File;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace kartverketprosjekt.Services.Sak
{
    public class SakService : ISakService
    {
        private readonly KartverketDbContext _context;
        private readonly IKommuneInfoService _kommuneInfoService;
        private readonly IFileService _fileService;
        private readonly DiscordBot _discordBot;

        public SakService(KartverketDbContext context, IKommuneInfoService kommuneInfoService, IFileService fileService, DiscordBot discordBot)
        {
            _context = context;
            _kommuneInfoService = kommuneInfoService;
            _fileService = fileService;
            _discordBot = discordBot;
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
        public async Task<List<SelectListItem>> GetCaseworkersAsync()
        {
            var caseworkers = await _context.Bruker
                .Where(b => b.tilgangsnivaa_id == 3)
                .Select(b => new SelectListItem { Value = b.epost, Text = b.navn })
                .ToListAsync();
            return caseworkers;
        }

        public async Task<int> RegisterCaseAsync(SakModel sak, IFormFile vedlegg, double nord, double ost, int koordsys, string currentUserEmail)
        {
            sak.epost_bruker = currentUserEmail;
            sak.kommune_id = 1;
            sak.status = "Ubehandlet";

            if (!string.IsNullOrEmpty(currentUserEmail))
            {
                var user = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == currentUserEmail);
                sak.IsPriority = user?.tilgangsnivaa_id == 2;
            }

            if (vedlegg != null)
            {
                sak.vedlegg = await _fileService.UploadFileAsync(vedlegg);
            }

            var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(nord, ost, koordsys);
            if (kommuneInfo != null)
            {
                sak.Kommunenavn = kommuneInfo.Kommunenavn;
                sak.Kommunenummer = kommuneInfo.Kommunenummer;
                sak.Fylkesnavn = kommuneInfo.Fylkesnavn;
                sak.Fylkesnummer = kommuneInfo.Fylkesnummer;
            }

            var leastAssignedCaseworker = await _context.Bruker
                .Where(b => b.tilgangsnivaa_id == 3)
                .OrderBy(b => _context.Sak.Count(s => s.SaksbehandlerId == b.epost))
                .FirstOrDefaultAsync();

            if (leastAssignedCaseworker != null)
            {
                sak.SaksbehandlerId = leastAssignedCaseworker.epost;
            }

            _context.Sak.Add(sak);
            await _context.SaveChangesAsync();

            await _discordBot.SendMessageToDiscord($"**En ny sak er opprettet i {sak.Kommunenavn}**\n**Beskrivelse:** {sak.beskrivelse}\n**Opprettet av:** {sak.epost_bruker}");

            return sak.id;
        }

        public SakModel GetCaseById(int id)
        {
            return _context.Sak.FirstOrDefault(s => s.id == id);
        }
    }
}
