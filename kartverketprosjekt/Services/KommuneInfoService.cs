using kartverketprosjekt.API_Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace kartverketprosjekt.Services
{
    public class KommuneInfoService : IKommuneInfoService
    {
        // Service for å hente kommune og fylke info
        private readonly HttpClient _httpClient;
        private readonly ILogger<KommuneInfoService> _logger;
        private readonly ApiSettings _apiSettings;

        public KommuneInfoService(HttpClient httpClient, ILogger<KommuneInfoService> logger, IOptions<ApiSettings> apisettings)
        {
            // Constructor for KommuneInfoService
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apisettings.Value;
        }
        public async Task<KommuneInfo> GetKommuneInfoAsync(double nord, double ost, int koordsys)
        {
            // Metode for å hente kommune og fylke info
            try
            {
                var response = await _httpClient.GetAsync($"{_apiSettings.KommuneInfoApiBaseUrl}punkt?nord={nord}&ost={ost}&koordsys={koordsys}");
                response.EnsureSuccessStatusCode();

                var json = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"KommuneInfo Response: {json}");
                var kommuneInfo = JsonSerializer.Deserialize<KommuneInfo>(json);
                return kommuneInfo;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error fetching kommune and fylke : {ex.Message}");
                return null;
            }

        }
    }
}
