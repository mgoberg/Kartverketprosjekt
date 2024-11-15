using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kartverketprosjekt.Repositories.Sak
{
    public interface ISakRepository
    {
        Task<int> GetCaseCountAsync();
        Task<int> GetCaseCountByStatusAsync(string status);
        Task<List<SakModel>> GetAllCasesAsync();
        Task<List<SelectListItem>> GetCaseworkersAsync();
        Task<int> RegisterCaseAsync(SakModel sak);
        SakModel GetCaseById(int id);
        Task UpdateStatus(int id, string status);
        Task AssignSaksbehandler(int sakId, string saksbehandlerEpost);
        List<SakModel> GetAllSaker();
        Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId);
        Task<List<SakModel>> GetUserCasesAsync(string userEmail);
        Task<bool> DeleteCaseAsync(int id);
        Task<BrukerModel> GetUserByEmailAsync(string email);
        Task<BrukerModel> GetLeastAssignedCaseworkerAsync();
        Task SaveChangesAsync();
    }
}

