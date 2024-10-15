using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace prosjekt_kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Liste som holder på alle posisjoner som blir lagt til

        private static List<PositionModel> positions = new List<PositionModel>();



        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
       
       

       


        /// Registers a new area change with the specified GeoJSON and description.
 
        // Action metode som håndterer GET request til /Home/CorrectMap
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

        // Action metode som håndterer GET request til /Home/CorrectionOverview
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
                // TODO: Håndter innsendt kontaktskjema
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

