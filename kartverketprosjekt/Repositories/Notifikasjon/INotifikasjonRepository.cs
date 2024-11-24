namespace kartverketprosjekt.Repositories.Notifikasjon
{
    public interface INotifikasjonRepository
    {
        Task<bool> HasStatusChangedAsync(string brukerEpost);
        Task<bool> ResetNotificationStatusAsync(string brukerEpost);

    }
}
