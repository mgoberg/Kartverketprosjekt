﻿using kartverketprosjekt.API_Models;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace kartverketprosjekt.Services.API
{
    public class KommuneInfoService : IKommuneInfoService
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<KommuneInfoService> _logger;
        private readonly ApiSettings _apiSettings;

        public KommuneInfoService(HttpClient httpClient, ILogger<KommuneInfoService> logger, IOptions<ApiSettings> apisettings)
        {
            _httpClient = httpClient;
            _logger = logger;
            _apiSettings = apisettings.Value;
        }

        /// <summary>
        /// Kaller Kartverkets Kommuneinfo API: <see href="https://api.kartverket.no/kommuneinfo/v1/"/> (get_punkt)
        /// </summary>
        /// <param name="nord"> nord/latitude-koordinaten</param>
        /// <param name="ost"> øst/longitude-koordinaten</param>
        /// <param name="koordsys"> Koordinatsystemet til koordinatene du søker med. Angis som en SRID, for eksempel 4258 eller 25833.</param>
        /// <returns>kommuneinfo som json-streng</returns>
        public async Task<KommuneInfo> GetKommuneInfoAsync(double nord, double ost, int koordsys)
        {
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
