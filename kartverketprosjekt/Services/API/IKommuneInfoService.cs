using kartverketprosjekt.API_Models;

namespace kartverketprosjekt.Services.API
{
    public interface IKommuneInfoService
    {
        // Interface for KommuneInfoService
        Task<KommuneInfo> GetKommuneInfoAsync(double nord, double ost, int koordsys);
    }
}
