using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services.Kommentar
{
    public interface IKommentarService
    {
        public Task AddComment(int sakID, string kommentar, string brukerEpost);

        public List<KommentarModel> GetComments(int sakId);
    }
}
