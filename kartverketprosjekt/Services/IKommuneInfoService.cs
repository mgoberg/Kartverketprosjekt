using kartverketprosjekt.API_Models;

namespace kartverketprosjekt.Services
{
    public interface IKommuneInfoService
    {
        Task<KommuneInfo> GetKommuneInfoAsync(string kommuneNr);
    }
}
