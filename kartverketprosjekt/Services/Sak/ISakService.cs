using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace kartverketprosjekt.Services.Sak
{
    public interface ISakService
    {
        public Task<List<SakModel>> GetUserCasesAsync(string userEmail);
        public Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId);
        public Task<bool> DeleteCaseAsync(int id);
        public Task<List<SelectListItem>> GetCaseworkersAsync();
        public Task<int> RegisterCaseAsync(SakModel sak, IFormFile vedlegg, double nord, double ost, int koordsys, string currentUserEmail);
        public SakModel GetCaseById(int id);
        public Task UpdateStatus(int id, string status);
        public Task DeleteCase(int id);
        public Task AssignSaksbehandler(int sakId, string saksbehandlerEpost);
        public List<SakModel> GetAllSaker();
    }
}
