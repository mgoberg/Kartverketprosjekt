namespace kartverketprosjekt.Services
{
    public interface INotifikasjonService
    {
        Task<bool> HarEndretStatus(string brukerEpost);

        Task<bool> ResetNotificationStatus(string brukerEpost);
    }
}
