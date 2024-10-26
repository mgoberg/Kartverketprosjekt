using kartverketprosjekt.API_Models;

namespace kartverketprosjekt.Services
{
    public interface IStedsnavnService //kan fjernes hvis ikke vi skal implementere stedsnavn api
    {
        Task<StedsnavnResponse> GetStedsnavnAsync(string search);
    }
}