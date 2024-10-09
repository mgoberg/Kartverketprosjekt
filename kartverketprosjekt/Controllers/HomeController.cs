using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace prosjekt_kartverket.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        // Liste som holder p� alle posisjoner som blir lagt til

        private static List<PositionModel> positions = new List<PositionModel>();

        private static List<AreaChangeModel> changes = new List<AreaChangeModel>();

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
        public IActionResult CaseWorkerView()
        {
            return View();
        }
        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }

       


        /// Registers a new area change with the specified GeoJSON and description.
        /// <returns>The action result for the area change overview view.</returns>
        [HttpPost]
        public IActionResult RegisterAreaChange(string geoJson, string description, string mapUrl)
        {
            var newChange = new AreaChangeModel
            {
                ID = Guid.NewGuid().ToString(), // genererer en unik ID for endringen
                GeoJSON = geoJson,
                Description = description,
                LayerUrl = mapUrl
            };
            changes.Add(newChange);
            

            return RedirectToAction("AreaChangeOverview");
        }

        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            return View(changes);
        }

        // Action metode som h�ndterer GET request til /Home/CorrectMap
        [HttpGet]
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
                // TODO: Process the form submission (e.g., send email, save to database)
                // For now, we'll just redirect to a thank you page
                return RedirectToAction("ThankYou");
            }

            // If the model is not valid, return to the form with validation errors
            return View(model);
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}

