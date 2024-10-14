
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace kartverketprosjekt.Controllers
{
    public class SakController : Controller
    {
        private readonly KartverketDbContext _context;

        public SakController(KartverketDbContext context)
        {
            _context = context;
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
    }
}
