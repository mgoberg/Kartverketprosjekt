using kartverketprosjekt.Models;

namespace kartverketprosjekt.Repositories.Kommentar
{
    public interface IKommentarRepository
    {
        Task AddCommentAsync(int sakID, string kommentar, string brukerEpost);
        Task<IEnumerable<KommentarModel>> GetCommentsAsync(int sakId);
    }

}
