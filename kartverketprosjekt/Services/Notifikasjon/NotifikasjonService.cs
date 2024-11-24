using kartverketprosjekt.Repositories.Notifikasjon;
using kartverketprosjekt.Services.Notifikasjon;

public class NotifikasjonService : INotifikasjonService
{
    private readonly INotifikasjonRepository _notifikasjonRepository;

    public NotifikasjonService(INotifikasjonRepository notifikasjonRepository)
    {
        _notifikasjonRepository = notifikasjonRepository;
    }

    /// <summary>
    /// Sjekker om brukeren har noen statusendringer som skal varsles om.
    /// </summary>
    /// <param name="brukerEpost">E-postadressen til brukeren.</param>
    /// <returns>boolsk verdi som indikerer om det er en statusendring.</returns>
    public async Task<bool> HarEndretStatus(string brukerEpost)
    {
        try
        {
            bool hasStatusChanged = await _notifikasjonRepository.HasStatusChangedAsync(brukerEpost);

            if (hasStatusChanged)
            {
                Console.WriteLine("Du har en notifikasjon.");
                return true; 
            }

            return false;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Feil ved henting av sak: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Tilbakestiller varslingsstatusen for en bruker.
    /// </summary>
    /// <param name="brukerEpost">E-postadressen til brukeren.</param>
    /// <returns>boolsk verdi som indikerer om tilbakestillingen var vellykket.</returns>
    public async Task<bool> ResetNotificationStatus(string brukerEpost)
    {
        try
        {
            return await _notifikasjonRepository.ResetNotificationStatusAsync(brukerEpost);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Feil ved tilbakestilling av notifikasjoner: {ex.Message}");
            return false;
        }
    }
}