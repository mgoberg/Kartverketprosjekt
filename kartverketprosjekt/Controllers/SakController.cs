
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using kartverketprosjekt.Services;

namespace kartverketprosjekt.Controllers
{
    public class SakController : Controller
    {
        private readonly KartverketDbContext _context;

        private readonly ILogger<HomeController> _logger;

        private readonly IKommuneInfoService _kommuneInfoService;

        private readonly IStedsnavnService _stedsnavnService;

        public SakController(KartverketDbContext context, ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService)
        {
            _context = context;
            _logger = logger;
            _kommuneInfoService = kommuneInfoService;
            _stedsnavnService = stedsnavnService;
        }
      

        [HttpGet]
        public IActionResult RegisterAreaChange()
        {
            return View();
        }
        [HttpPost]
        public IActionResult RegisterAreaChange(SakModel sak, IFormFile vedlegg)
        {
            // Hent e-post fra den innloggede brukeren
            sak.epost_bruker = User.Identity.Name;

            // Hardkodede verdier for kommune_id og status
            sak.kommune_id = 1;
            sak.status = "Påbegynt";

            // Sjekk om vedlegg er lastet opp
            if (vedlegg != null && vedlegg.Length > 0)
            {
                // Define the path to save the file
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Ensure the uploads directory exists
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Generate a unique file name
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vedlegg.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                // Save the file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    vedlegg.CopyTo(stream);
                }

                // Store the file path (relative to wwwroot) in the SakModel
                sak.vedlegg = fileName; // Change this if you have a different property type
            }

            // Lagre saken i databasen
            _context.Sak.Add(sak);
            _context.SaveChanges();

            TempData["id"] = sak.id;

            // Viderefør til oversiktsiden
            return RedirectToAction("AreaChangeOverview");
        }


        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            // Retrieve the ID from TempData
            if (TempData.ContainsKey("id"))
            {
                int id = (int)TempData["id"];

                // Hent den spesifikke saken basert på id
                var sak = _context.Sak.FirstOrDefault(s => s.id == id);

                if (sak == null)
                {
                    return NotFound(); // Håndterer hvis saken ikke finnes
                }

                return View(sak); // Sender den innsendte saken til viewet
            }

            return NotFound(); // Handle case where no ID is found in TempData
        }

        public IActionResult ApiIndex()
        {
            return View();
        }

        //// Handles the search for KommuneInfo
        //[HttpPost]
        //public async Task<IActionResult> KommuneInfo(string kommuneNr)
        //{
        //    if (string.IsNullOrEmpty(kommuneNr))
        //    {
        //        ViewData["Error"] = "Please enter a valid Kommune Number.";
        //        return View("ApiIndex");
        //    }

        //    var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(kommuneNr);
        //    if (kommuneInfo != null)
        //    {
        //        var viewModel = new KommuneInfoViewModel
        //        {
        //            Kommunenavn = kommuneInfo.Kommunenavn,
        //            Kommunenummer = kommuneInfo.Kommunenummer,
        //            Fylkesnavn = kommuneInfo.Fylkesnavn,
        //            SamiskForvaltningsomrade = kommuneInfo.SamiskForvaltningsomrade
        //        };
        //        return View("KommuneInfo", viewModel);
        //    }
        //    else
        //    {
        //        ViewData["Error"] = $"No results found for Kommune Number '{kommuneNr}'.";
        //        return View("ApiIndex");
        //    }
        //}
        [HttpPost]
        public async Task<IActionResult> KommuneInfo(double nord, double ost, int koordsys)
        {
            //if (string.IsNullOrEmpty(nord, ost))
            //{
            //    ViewData["Error"] = "Please enter a valid Kommune Number.";
            //    return View("ApiIndex");
            //}

            var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(nord, ost, koordsys);
            if (kommuneInfo != null)
            {
                var viewModel = new KommuneInfoViewModel
                {
                    Kommunenavn = kommuneInfo.Kommunenavn,
                    Kommunenummer = kommuneInfo.Kommunenummer,
                    Fylkesnavn = kommuneInfo.Fylkesnavn,
                    Fylkesnummer = kommuneInfo.Fylkesnummer,
                };
                return View("KommuneInfo", viewModel);
            }
            else
            {
                ViewData["Error"] = $"No results found for coordinates.";
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
    }
}
