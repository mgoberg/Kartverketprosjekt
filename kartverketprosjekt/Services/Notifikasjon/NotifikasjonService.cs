using kartverketprosjekt.Repositories.Notifikasjon;
using kartverketprosjekt.Services.Notifikasjon;

public class NotifikasjonService : INotifikasjonService
{
    private readonly INotifikasjonRepository _notifikasjonRepository;

    public NotifikasjonService(INotifikasjonRepository notifikasjonRepository)
    {
        _notifikasjonRepository = notifikasjonRepository;
    }

    // Check if the user has any status changes to be notified about
    public async Task<bool> HarEndretStatus(string brukerEpost)
    {
        try
        {
            bool hasStatusChanged = await _notifikasjonRepository.HasStatusChangedAsync(brukerEpost);

            if (hasStatusChanged)
            {
                // Log for debugging purposes
                Console.WriteLine("Du har en notifikasjon.");
                return true; // Return true for notification
            }

            return false; // No notification
        }
        catch (Exception ex)
        {
            // Log error
            Console.WriteLine($"Feil ved henting av sak: {ex.Message}");
            return false;
        }
    }

    // Reset the notification status for a user
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