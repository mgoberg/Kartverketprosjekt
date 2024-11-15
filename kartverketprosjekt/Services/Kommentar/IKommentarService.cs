using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services.Kommentar
{
    public interface IKommentarService
    {
        Task AddComment(int sakID, string kommentar, string brukerEpost);
        Task<List<KommentarModel>> GetComments(int sakId);
    }

}
