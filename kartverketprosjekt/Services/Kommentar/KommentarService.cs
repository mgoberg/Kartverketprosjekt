using kartverketprosjekt.Repositories.Kommentar;
using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services.Kommentar
{

    /// <summary>
    /// Tjeneste for å håndtere kommentarer.
    /// </summary>
    public class KommentarService : IKommentarService
    {
        private readonly IKommentarRepository _kommentarRepository;

        /// <summary>
        /// Initialiserer en ny instans av <see cref="KommentarService"/> klassen.
        /// </summary>
        /// <param name="kommentarRepository">Repository for kommentarer.</param>
        public KommentarService(IKommentarRepository kommentarRepository)
        {
            _kommentarRepository = kommentarRepository;
        }

        /// <summary>
        /// Legger til en kommentar til en sak.
        /// </summary>
        /// <param name="sakID">ID til saken.</param>
        /// <param name="kommentar">Teksten til kommentaren.</param>
        /// <param name="brukerEpost">E-postadressen til brukeren som legger til kommentaren.</param>
        /// <returns>En oppgave som representerer den asynkrone operasjonen.</returns>
        /// <exception cref="ArgumentException">Kastes hvis sakID er mindre enn eller lik 0, eller hvis kommentaren er tom.</exception>
        public async Task AddComment(int sakID, string kommentar, string brukerEpost)
        {
            if (sakID <= 0 || string.IsNullOrWhiteSpace(kommentar))
                throw new ArgumentException("Ugyldig sakID eller kommentar.");

            await _kommentarRepository.AddCommentAsync(sakID, kommentar, brukerEpost);
        }

        /// <summary>
        /// Henter kommentarer for en gitt sak.
        /// </summary>
        /// <param name="sakId">ID til saken.</param>
        /// <returns>En liste over kommentarer for den gitte saken.</returns>
        /// <exception cref="ArgumentException">Kastes hvis sakID er mindre enn eller lik 0.</exception>
        public async Task<List<KommentarModel>> GetComments(int sakId)
        {
            if (sakId <= 0)
            {
                throw new ArgumentException("invalid SakID.");
            }

            return (List<KommentarModel>)await _kommentarRepository.GetCommentsAsync(sakId);
        }
    }
}