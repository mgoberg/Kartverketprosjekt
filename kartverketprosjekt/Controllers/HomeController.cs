using kartverketprosjekt.Models;
using kartverketprosjekt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace kartverketprosjekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly IKommuneInfoService _kommuneInfoService;

        private readonly IStedsnavnService _stedsnavnService;


        // Liste som holder p� alle posisjoner som blir lagt til
        private static List<PositionModel> positions = new List<PositionModel>();



        public HomeController(ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService)
        {
            _logger = logger;
            _kommuneInfoService = kommuneInfoService;
            _stedsnavnService = stedsnavnService;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ApiIndex()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult MyPage()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        // Handles the search for KommuneInfo
        [HttpPost]
        public async Task<IActionResult> KommuneInfo(string kommuneNr)
        {
            if (string.IsNullOrEmpty(kommuneNr))
            {
                ViewData["Error"] = "Please enter a valid Kommune Number.";
                return View("ApiIndex");
            }

            var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(kommuneNr);
            if (kommuneInfo != null)
            {
                var viewModel = new KommuneInfoViewModel
                {
                    Kommunenavn = kommuneInfo.Kommunenavn,
                    Kommunenummer = kommuneInfo.Kommunenummer,
                    Fylkesnavn = kommuneInfo.Fylkesnavn,
                    SamiskForvaltningsomrade = kommuneInfo.SamiskForvaltningsomrade
                };
                return View("KommuneInfo", viewModel);
            }
            else
            {
                ViewData["Error"] = $"No results found for Kommune Number '{kommuneNr}'.";
                return View("ApiIndex");
            }
        }

        // Handles the search for Stedsnavn
        [HttpPost]
        public async Task<IActionResult> Stedsnavn(string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm))
            {
                ViewData["Error"] = "Please enter a valid place name.";
                return View("ApiIndex");
            }

            var stedsnavnResponse = await _stedsnavnService.GetStedsnavnAsync(searchTerm);
            if (stedsnavnResponse?.Navn != null && stedsnavnResponse.Navn.Any())
            {
                var viewModel = stedsnavnResponse.Navn.Select(n => new StedsnavnViewModel
                {
                    Skrivemåte = n.Skrivemåte,
                    Navneobjekttype = n.Navneobjekttype,
                    Språk = n.Språk,
                    Navnestatus = n.Navnestatus
                }).ToList();

                return View("Stedsnavn", viewModel);
            }
            else
            {
                ViewData["Error"] = $"No results found for '{searchTerm}'.";
                return View("ApiIndex");
            }
        }





        /// Registers a new area change with the specified GeoJSON and description.

        // Action metode som h�ndterer GET request til /Home/CorrectMap
        [HttpGet]
        [AllowAnonymous]
        public IActionResult CorrectMap()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CorrectMap(PositionModel model)
        {
            if (ModelState.IsValid)
            {
                // Legger ny posisjon til "positions" listen
                positions.Add(model);

                // Viser oppsummering view etter data har blitt registrert og lagret i positions listen
                return View("CorrectionOverview", positions);
            }
            return View();
        }

        // Action metode som h�ndterer GET request til /Home/CorrectionOverview
        [HttpGet]
        public IActionResult CorrectionOverview()
        {
            return View(positions);
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // New Contact-related actions
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: H�ndter innsendt kontaktskjema
                return RedirectToAction("ContactConfirmed");
            }

            // If the model is not valid, return to the form with validation errors
            return View(model);
        }

        public IActionResult ContactConfirmed()
        {
            return View();
        }
    }
}

