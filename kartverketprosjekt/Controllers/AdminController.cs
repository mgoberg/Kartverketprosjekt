﻿using kartverketprosjekt.Data;
using Microsoft.AspNetCore.Mvc;

namespace kartverketprosjekt.Controllers
{
    public class AdminController : Controller
    {
        private readonly KartverketDbContext _context;

        public AdminController(KartverketDbContext context)
        {
            _context = context;
        }
        public IActionResult AdminView()
        {
            var users = _context.Bruker.ToList(); // Hent alle brukere fra databasen
            return View(users); // Send brukerne til viewet
        }

        [HttpPost]
        public IActionResult UpdateAccess(string userId, int newAccessLevel)
        {
            var user = _context.Bruker.Find(userId);
            if (user != null)
            {
                user.tilgangsnivaa_id = newAccessLevel; // Oppdater tilgangsnivået
                _context.SaveChanges(); // Lagre endringene i databasen
            }
            return RedirectToAction("AdminView"); // Naviger tilbake til listen over brukere
        }

        [HttpPost]
        public IActionResult SlettBruker(string epost)
        {
            if (string.IsNullOrEmpty(epost))
            {
                return BadRequest("E-post må være oppgitt.");
            }

            // Finn brukeren med den gitte e-posten
            var bruker = _context.Bruker.FirstOrDefault(u => u.epost == epost);
            if (bruker == null)
            {
                return NotFound("Bruker ikke funnet.");
            }

            // Sjekk om brukeren har tilknyttede saker
            var saker = _context.Sak.Where(s => s.epost_bruker == bruker.epost).ToList();

            if (saker.Any())
            {
                // Hvis brukeren har saker, returner til adminvisningen med en feilmelding
                ViewBag.ErrorMessage = "Brukeren kan ikke slettes fordi det finnes saker knyttet til denne brukeren.";
                var alleBrukere = _context.Bruker.ToList(); // Hent alle brukere for å vise i tabellen
                return View("AdminView", alleBrukere); // Returner til adminvisningen med alle brukere
            }

            // Hvis brukeren ikke har saker, fjern brukeren fra databasen
            _context.Bruker.Remove(bruker);
            _context.SaveChanges(); // Lagre endringene

            return RedirectToAction("AdminView"); // Naviger tilbake til listen over brukere
        }
    }
}