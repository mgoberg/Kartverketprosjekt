using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Sak;
using kartverketprosjekt.Services.API;
using kartverketprosjekt.Services.File;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace kartverketprosjekt.Services.Sak
{
    

    public class SakService : ISakService
    {
        private readonly ISakRepository _sakRepository;
        private readonly IKommuneInfoService _kommuneInfoService;
        private readonly IFileService _fileService;
        private readonly DiscordBot _discordBot;

        public SakService(ISakRepository sakRepository, IKommuneInfoService kommuneInfoService, IFileService fileService, DiscordBot discordBot)
        {
            _sakRepository = sakRepository;
            _kommuneInfoService = kommuneInfoService;
            _fileService = fileService;
            _discordBot = discordBot;
        }

        public async Task<List<SakModel>> GetUserCasesAsync(string userEmail)
        {
            return await _sakRepository.GetUserCasesAsync(userEmail);
        }

        public async Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId)
        {
            return await _sakRepository.GetCommentsAsync(sakId);
        }

        public async Task<bool> DeleteCaseAsync(int id)
        {
            return await _sakRepository.DeleteCaseAsync(id);
        }

        public async Task<List<SelectListItem>> GetCaseworkersAsync()
        {
            return await _sakRepository.GetCaseworkersAsync();
        }

        public async Task<int> RegisterCaseAsync(SakModel sak, IFormFile vedlegg, double nord, double ost, int koordsys, string currentUserEmail)
        {
            sak.epost_bruker = currentUserEmail;
            sak.kommune_id = 1;
            sak.status = "Ubehandlet";

            if (!string.IsNullOrEmpty(currentUserEmail))
            {
                var user = await _sakRepository.GetUserByEmailAsync(currentUserEmail);
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

            var leastAssignedCaseworker = await _sakRepository.GetLeastAssignedCaseworkerAsync();
            if (leastAssignedCaseworker != null)
            {
                sak.SaksbehandlerId = leastAssignedCaseworker.epost;
            }

            var caseId = await _sakRepository.RegisterCaseAsync(sak);

            await _discordBot.SendMessageToDiscord($"**En ny sak er opprettet i {sak.Kommunenavn}**\n**Beskrivelse:** {sak.beskrivelse}\n**Opprettet av:** {sak.epost_bruker}");

            return caseId;
        }

        public SakModel GetCaseById(int id)
        {
            return _sakRepository.GetCaseById(id);
        }

        public async Task UpdateStatus(int id, string status)
        {
            await _sakRepository.UpdateStatus(id, status);
        }

        public async Task DeleteCase(int id)
        {
            await _sakRepository.DeleteCaseAsync(id);
        }

        public async Task AssignSaksbehandler(int sakId, string saksbehandlerEpost)
        {
            await _sakRepository.AssignSaksbehandler(sakId, saksbehandlerEpost);
        }

        public List<SakModel> GetAllSaker()
        {
            return _sakRepository.GetAllSaker();
        }
    }
}

