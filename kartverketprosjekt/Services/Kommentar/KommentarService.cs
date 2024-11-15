using kartverketprosjekt.Repositories.Kommentar;
using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services.Kommentar
{

    public class KommentarService : IKommentarService
    {
        private readonly IKommentarRepository _kommentarRepository;

        public KommentarService(IKommentarRepository kommentarRepository)
        {
            _kommentarRepository = kommentarRepository;
        }

        public async Task AddComment(int sakID, string kommentar, string brukerEpost)
        {
            if (sakID <= 0 || string.IsNullOrWhiteSpace(kommentar))
                throw new ArgumentException("Ugyldig sakID eller kommentar.");

            await _kommentarRepository.AddCommentAsync(sakID, kommentar, brukerEpost);
        }

        public async Task<List<KommentarModel>> GetComments(int sakId)
        {
            return (List<KommentarModel>)await _kommentarRepository.GetCommentsAsync(sakId);
        }
    }
}