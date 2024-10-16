using Microsoft.AspNetCore.Mvc;

namespace kartverketprosjekt.API_Models
{
    public class ApiSettings : Controller
    {
        public string KommuneInfoApiBaseUrl { get; set; }
        public string StedsnavnApiBaseUrl { get; set; }
    }   
}
