using kartverketprosjekt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using kartverketprosjekt.Models;

namespace kartverketprosjekt.Controllers

{
    [Authorize(Roles = "4")]
    public class AdminController : Controller
    {
        private readonly KartverketDbContext _context;

        public AdminController(KartverketDbContext context)
        {
            _context = context;
        }
        public IActionResult AdminView()
        {
            // Retrieve the total count of cases (saker) from the database
            int caseCount = _context.Sak.Count();

            // Retrieve the total count of users from the database
            int userCount = _context.Bruker.Count();

            // Optional: Additional stats, like cases by status or date range
            int openCasesUnbehandlet = _context.Sak.Count(s => s.status == "Ubehandlet");
            int openCasesUnderBehandling = _context.Sak.Count(s => s.status == "Under Behandling");
            int openCasesAvvist = _context.Sak.Count(s => s.status == "Avvist");
            int openCasesArkivert = _context.Sak.Count(s => s.status == "Arkivert");
            int closedCases = _context.Sak.Count(s => s.status == "Løst");

            // Pass the counts to the view using ViewData or a view model
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

        [HttpPost]
        public IActionResult UpdateAccess(string userId, int newAccessLevel)
        {
            var user = _context.Bruker.Find(userId);
            if (user != null)
            {
                user.tilgangsnivaa_id = newAccessLevel; // Oppdater tilgangsnivået
                _context.SaveChanges(); // Lagre endringene i databasen
                TempData["SuccessMessage"] = $"Endret tilgangsnivå for {userId} til: {newAccessLevel}"; // Set success message

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

        [HttpPost]
        public IActionResult OpprettBruker(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn)
        {
            if (string.IsNullOrEmpty(epost) || string.IsNullOrEmpty(passord) || string.IsNullOrEmpty(organisasjon))
            {
                ViewBag.ErrorMessage = "E-post, passord og organisasjon må fylles ut.";
                return RedirectToAction("AdminView");
            }

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

            TempData["SuccessMessage"] = $"Bruker med e-post {epost} ble opprettet.";
            return RedirectToAction("AdminView");
        }
    }


}
