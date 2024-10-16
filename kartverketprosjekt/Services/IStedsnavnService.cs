using kartverketprosjekt.API_Models;

namespace kartverketprosjekt.Services
{
    public interface IStedsnavnService
    {
        Task<StedsnavnResponse> GetStedsnavnAsync(string search);
    }
}