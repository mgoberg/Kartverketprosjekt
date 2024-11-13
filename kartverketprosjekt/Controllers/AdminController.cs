using kartverketprosjekt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using kartverketprosjekt.Models;

// ****************************************************************************************************************************
// ******AdminController er en controller som håndterer alle funksjoner som kun skal være tilgjengelig for Administrator.******
// ****************************************************************************************************************************

namespace kartverketprosjekt.Controllers

{


    // Gir tilgang til AdminController kun for brukere med rolle 4 (admin brukere)
    [Authorize(Roles = "4")]
    public class AdminController : Controller
    {
        // Dependency injection for å få tilgang til databasen.
        private readonly KartverketDbContext _context;

        // Konstruktør for AdminController
        public AdminController(KartverketDbContext context)
        {
            _context = context;
        }

        // Metode for å vise admin viewet.
        public IActionResult AdminView()
        {
            // Henter alle saker fra databasen og teller de.
            int caseCount = _context.Sak.Count();

            // Henter alle brukere fra databasen og teller de.
            int userCount = _context.Bruker.Count();

            // Henter ut stats for antall saker i de forskjellige statusene.
            int openCasesUnbehandlet = _context.Sak.Count(s => s.status == "Ubehandlet");
            int openCasesUnderBehandling = _context.Sak.Count(s => s.status == "Under Behandling");
            int openCasesAvvist = _context.Sak.Count(s => s.status == "Avvist");
            int openCasesArkivert = _context.Sak.Count(s => s.status == "Arkivert");
            int closedCases = _context.Sak.Count(s => s.status == "Løst");

            // Bruker ViewData for å sende data til viewet.
            ViewData["CaseCount"] = caseCount;
            ViewData["UserCount"] = userCount;
            ViewData["OpenCasesUnbehandlet"] = openCasesUnbehandlet;
            ViewData["OpenCasesUnderBehandling"] = openCasesUnderBehandling;
            ViewData["OpenCasesAvvist"] = openCasesAvvist;
            ViewData["OpenCasesArkivert"] = openCasesArkivert;
            ViewData["ClosedCases"] = closedCases;

            var users = _context.Bruker.ToList(); // Hent alle brukere fra databasen
            return View(users); // Send brukerne til viewet
        }

        //Metode for å endre tilgangsnivået til en bruker fra admin bruker.
        [HttpPost]
        public IActionResult UpdateAccess(string userId, int newAccessLevel)
        {
            // Finner Bruker med bruker med id.
            var user = _context.Bruker.Find(userId);
            if (user != null)
            {
                user.tilgangsnivaa_id = newAccessLevel; // Oppdater tilgangsnivået
                _context.SaveChanges(); // Lagre endringene i databasen
                TempData["SuccessMessage"] = $"Endret tilgangsnivå for {userId} til: {newAccessLevel}"; // Set success message

            }
            return RedirectToAction("AdminView"); // Naviger tilbake til listen over brukere
        }

           // Metode for å slette en bruker fra admin brukeren.
        [HttpPost]
        public IActionResult SlettBruker(string epost)
        { 
            // Sjekk om e-post er fylt ut
            if (string.IsNullOrEmpty(epost))
            {
                return BadRequest("E-post må være oppgitt.");
            }

            // Hent den innloggede brukerens e-post
            var innloggetBrukerEpost = User.Identity.Name;

            // Finn brukeren med den gitte e-posten
            var bruker = _context.Bruker.FirstOrDefault(u => u.epost == epost);
            if (bruker == null)
            {
                return NotFound("Bruker ikke funnet.");
            }

            // Sjekk om den innloggede brukeren prøver å slette sin egen konto
            if (bruker.epost == innloggetBrukerEpost)
            {
                ViewBag.ErrorMessage = "Du kan ikke slette din egen konto.";
                var alleBrukere = _context.Bruker.ToList(); // Hent alle brukere for å vise i tabellen
                return View("AdminView", alleBrukere); // Returner til adminvisningen med alle brukere
            }

            // Sjekk om brukeren har tilknyttede saker
            var saker = _context.Sak.Where(s => s.epost_bruker == bruker.epost).ToList();

            if (saker.Any())
            {
                // Hvis brukeren har saker, returner til adminvisningen med en feilmelding
                ViewBag.ErrorMessage = "Brukeren kan ikke slettes fordi det finnes saker knyttet til denne brukeren (benytt saksbehandler view til å fjerne saker).";
                var alleBrukere = _context.Bruker.ToList(); // Hent alle brukere for å vise i tabellen
                return View("AdminView", alleBrukere); // Returner til adminvisningen med alle brukere
            }

            // Hvis brukeren ikke har saker, fjern brukeren fra databasen
            _context.Bruker.Remove(bruker);
            _context.SaveChanges(); // Lagre endringene

            TempData["SuccessMessage"] = $"{bruker.epost} ble slettet.";

            return RedirectToAction("AdminView"); // Naviger tilbake til listen over brukere

        }

        // Metode for å opprette en ny bruker fra admin brukeren.
        [HttpPost]
        public IActionResult OpprettBruker(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn)
        {
            // Sjekk om e-post, passord og organisasjon er fylt ut.
            if (string.IsNullOrEmpty(epost) || string.IsNullOrEmpty(passord) || string.IsNullOrEmpty(organisasjon))
            {
                ViewBag.ErrorMessage = "E-post, passord og organisasjon må fylles ut.";
                return RedirectToAction("AdminView");
            }
            // Sjekk om e-posten allerede eksisterer i databasen
            if (_context.Bruker.Any(b => b.epost == epost))
            {
                ViewBag.ErrorMessage = "En bruker med denne e-posten eksisterer allerede.";
                return RedirectToAction("AdminView");
            }

            // Hash passordet ved hjelp av PasswordHasher
            var passwordHasher = new PasswordHasher<BrukerModel>();
            var nyBruker = new BrukerModel
            {
                epost = epost,
                passord = passwordHasher.HashPassword(null, passord),
                tilgangsnivaa_id = tilgangsnivaa,
                organisasjon = organisasjon,
                navn = navn // Navn kan være null
            };

            // Legg til ny bruker i databasen
            _context.Bruker.Add(nyBruker);
            _context.SaveChanges();
            // Sett en suksessmelding
            TempData["SuccessMessage"] = $"Bruker med e-post {epost} ble opprettet.";
            return RedirectToAction("AdminView");
        }
    }


}
