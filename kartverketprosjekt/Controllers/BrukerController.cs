using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
// ***********************************************************************************************************************
// ******BrukerController er en controller som håndterer alle funksjoner som kun skal være tilgjengelig for brukere.******
// ***********************************************************************************************************************

public class BrukerController : Controller
{
    
    private readonly KartverketDbContext _context;

    public BrukerController(KartverketDbContext context)
    {
        _context = context;
    }

    // Viser min side.
    public async Task<IActionResult> MyPage()
    {
        // Hent e-post fra den innloggede brukeren
        var bruker = User.Identity.Name;

        // Legg til en sjekk for null
        if (string.IsNullOrEmpty(bruker))
        {
            return RedirectToAction("Index", "Home"); // Hvis brukeren ikke er innlogget, omdiriger
        }

        Console.WriteLine($"Brukerens e-post: {bruker}"); // DEBUG

        // Hent saker knyttet til brukeren fra databasen
        var saker = await _context.Sak
            .Where(s => s.epost_bruker == bruker)
            .ToListAsync();

        // Send sakene til viewet
        return View(saker);
    }

    // POST login
    // Metode for å logge inn en bruker.
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Login(string epost, string password)
    {
        // Henter brukeren fra databasen basert på e-post.
        var user = _context.Bruker.FirstOrDefault(u => u.epost == epost);
        // Sjekker om brukeren finnes og om passordet er korrekt.
        if (user != null)
        {
            var passwordHasher = new PasswordHasher<BrukerModel>();

            // Sjekker om passordet er hashet eller ei.
            bool isPasswordValid;
            // hvis passordet er hashet så verifiseres det.
            if (IsHashedPassword(user.passord))
            {
                // Verifiserer
                var result = passwordHasher.VerifyHashedPassword(user, user.passord, password);
                isPasswordValid = result == PasswordVerificationResult.Success;
            }
            else
            {
                // Sammenlikner klartekst passord
                isPasswordValid = user.passord == password;
            }
            // Hvis passordet er korrekt så logges brukeren inn.
            if (isPasswordValid)
            {
                // Oppretter Claims for brukeren med epost som navn og tilgangsnivå som rolle.
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.epost), // Håndterer navnet.
                new Claim(ClaimTypes.Role, user.tilgangsnivaa_id.ToString()) // Håndterer rollen.
            };
                // ClaimsIdentity representerer brukerens identitet og tilhørende krav, som navn og rolle. Bruker Cookies for autentisering.
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                // Logger inn brukere med CookieAuthenticationDefaults.AuthenticationScheme.
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("RegisterAreaChange", "Sak");
            }
        }

        // hvis brukeren ikke finnes eller passordet er feil så vises en feilmelding.
        TempData["ErrorMessage"] = "Feil epost eller passord";
        return RedirectToAction("Index", "Home");
    }

    private bool IsHashedPassword(string password)
    {
        // Passordet hashes med PasswordHasher på en slik måte at det er lengre enn 60 tegn.
        return password.Length >= 60;
    }

    // Metode for å logge ut en bruker.
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        // Logger ut brukeren.
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Set a success message for the logout
        TempData["Message"] = "You have successfully logged out.";
        return RedirectToAction("Index", "Home"); // Redirect to the desired page after logout
    }


   
    // Metode for å registrere en ny bruker.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrerModel model)
    {
        if (ModelState.IsValid)
        {
            // Oppretter en instans av PasswordHasher for å hashe passordet.
            var passwordHasher = new PasswordHasher<BrukerModel>();

            // Setter verdier og hasher passordet.
            var bruker = new BrukerModel
            {
                // Passordet hashes med PasswordHasher
                passord = passwordHasher.HashPassword(null, model.passord),
                epost = model.epost,
                navn = model.navn,
                tilgangsnivaa_id = 1 // Setter tilgangsnivå til 1 som standard.
            };

            // Nullstiller passord før lagring slik at passordet ikke finnes i klartekst.
            model.passord = null;

            // Legger til bruker i databasen.
            await _context.AddAsync(bruker);
            await _context.SaveChangesAsync();

            // Logger inn bruker etter registrering.
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, bruker.epost),
            new Claim(ClaimTypes.Role, bruker.tilgangsnivaa_id.ToString()) // Håndterer roller.
        };

    
            // ClaimsIdentity representerer brukerens identitet og tilhørende krav, som navn og rolle.
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // CookieAuthenticationDefaults.AuthenticationScheme angir at vi bruker cookie-basert autentisering.
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            // Feedback på vellykket registrering
            TempData["Message"] = $"Hei {model.navn}, registreringen er vellykket!";

            // Tar brukeren til kartet.
            return RedirectToAction("RegisterAreaChange", "Sak");
            
        }

        // Hvis modellen ikke er gyldig, vis feilmelding.
        TempData["ErrorMessage"] = "En feil oppstod under registreringen.";
        return RedirectToAction("Index", "Home");
    }


    // Metode for å oppdatere passordet til en bruker / Endre passord.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePassword(string password)
    {
        // Henter e-post fra den innloggede brukeren. (Brukernavnet / Brukeren)
        string epost = User.Identity.Name;

        // Finner brukeren i databasen.
        var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);

        // Sjekker om brukeren finnes.
        if (bruker != null)
        {
            var passwordHasher = new PasswordHasher<BrukerModel>();
            bruker.passord = passwordHasher.HashPassword(bruker, password); // Hasher passordet.

            // Lagrer endringene i databasen.
            await _context.SaveChangesAsync();

            // Feedback på vellykket endring av passord.
            TempData["SuccessMessage"] = "Passordet er oppdatert.";
            return RedirectToAction("Mypage");
        }
        // Hvis brukeren ikke finnes, vis feilmelding.
        TempData["ErrorMessage"] = "Kunne ikke oppdatere passordet.";
        return RedirectToAction("Mypage");
    }

    // Metode for å oppdatere navnet til en bruker.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateName(string navn)
    {
        // Henter e-post fra den innloggede brukeren. (Brukernavnet / Brukeren)
        string epost = User.Identity.Name;

        var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
        if (bruker != null)
        {
            bruker.navn = navn; // Oppdaterer navnet til brukeren

            // Lagrer endringene i databasen
            await _context.SaveChangesAsync();

            // Feedback på vellykket endring av navn
            TempData["SuccessMessage"] = "Navnet er oppdatert.";
            return RedirectToAction("Mypage");
        }

        TempData["ErrorMessage"] = "Kunne ikke oppdatere navnet.";
        return RedirectToAction("Mypage");
    }

    // Metode for å slette en bruker.
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string passord)
    {

        // Prøver å slette brukeren.
        try
        {
            // Hent e-post fra den påloggede brukeren.
            string epost = User.Identity.Name;

            // Finn brukeren i databasen.
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
            if (bruker == null)
            {
                TempData["ErrorMessage"] = "Bruker finnes ikke.";
                Console.WriteLine("User not found"); // DEBUG
                return RedirectToAction("Mypage");
            }

            // Sjekk om passordet er korrekt
            var passwordHasher = new PasswordHasher<BrukerModel>();
            bool isPasswordValid;

            if (IsHashedPassword(bruker.passord))
            {
                // Verfier det hashede passordet.
                var result = passwordHasher.VerifyHashedPassword(bruker, bruker.passord, passord);
                isPasswordValid = result == PasswordVerificationResult.Success;
            }
            else
            {
                // Samenlikner klartekst passord.
                isPasswordValid = bruker.passord == passord;
            }

            // Hvis passordet er feil, vis feilmelding.
            if (!isPasswordValid)
            {
                TempData["ErrorMessage"] = "Feil passord. Vennligst prøv igjen.";
                Console.WriteLine("Invalid password");
                return RedirectToAction("Mypage");
            }

            // Slett brukeren
            _context.Bruker.Remove(bruker);
            await _context.SaveChangesAsync();
            Console.WriteLine("User deleted successfully");
            // Logg ut brukeren.
            await HttpContext.SignOutAsync();

            TempData["SuccessMessage"] = "Brukeren er slettet.";
            return RedirectToAction("Index", "Home"); // Redirect til ønsket side etter sletting
        }
        // Håndter eventuelle feil.
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            TempData["ErrorMessage"] = "Det oppstod en feil ved sletting av brukeren.";
            return RedirectToAction("Mypage");
        }
    }

    // Metode for å slette en sak.
    [HttpPost]
    public IActionResult DeleteCase(int id)
    {
        // Henter saken fra databasen basert på id.
        var sak = _context.Sak.Include(s => s.Kommentarer).FirstOrDefault(s => s.id == id);

        // Sjekker om saken finnes.
        if (sak != null)
        {
            // Fjerner kommentarer assosiert med saken.
            _context.Kommentar.RemoveRange(sak.Kommentarer);

            // Fjerner saken fra db.
            _context.Sak.Remove(sak);
            _context.SaveChanges();

            // Sukessmelding.
            TempData["SuccessMessage"] = $"Sak: {id} ble slettet";

            return RedirectToAction("Mypage");
        }

        // Error melding.
        TempData["ErrorMessage"] = "Kunne ikke slette sak";

        return RedirectToAction("Mypage");
    }

    // Metode for å sjekke om en saksbehandler har endret status på saken sin
    [HttpGet]
    public async Task<IActionResult> HarEndretStatus()
    {
        // Sjekker
        try
        {
            // Sjekk om det finnes minst én sak hvor brukeren har endret status
            var hasStatusChanged = await _context.Sak
                .AnyAsync(s => s.epost_bruker == User.Identity.Name && s.status_endret == true);

            if (hasStatusChanged)
            {
                // Logg i konsollen for debugging
                Console.WriteLine("Du har en notifikasjon.");

                return Json(true); // Returner true for å indikere at det er en notifikasjon
            }

            // Hvis ingen sak har status_endret == true
            return Json(false); // Returner false for å indikere at det ikke er noen notifikasjon
        }
        catch (Exception ex)
        {
            // Håndter eventuelle feil ved henting
            Console.WriteLine($"Feil ved henting av sak: {ex.Message}");
            return Json(false); // Returner false hvis det skjer en feil
        }
    }


    // Metode for å nullstille status_endret for alle saker som er relatert til brukeren
    [HttpPost]
    public async Task<IActionResult> ResetNotificationStatus()
    {
        // Hent alle saker som er relatert til brukeren og har endret status.
        var saker = await _context.Sak
            .Where(s => s.epost_bruker == User.Identity.Name && s.status_endret)
            .ToListAsync();

        // Sett statusEndret tilbake til false.
        foreach (var sak in saker)
        {
            sak.status_endret = false;
        }

        await _context.SaveChangesAsync(); // Lagre endringene til databasen.

        return Json(new { success = true });
    }

    // Metode for å hente kommentarer til en sak.
    // Spesifik routing for å hente kommentarer til en sak.
    [Route("Bruker/GetComments")]
    [HttpGet]
    public IActionResult GetComments(int sakId)
    {
        // Hent alle kommentarer til en sak basert på sakId.
        var kommentarer = _context.Kommentar.Where(c => c.SakID == sakId).ToList();
        // Sjekk om det finnes kommentarer.
        if (kommentarer != null && kommentarer.Any())
        {
            return Json(new { success = true, kommentarer = kommentarer });
        }
        // Hvis det ikke finnes kommentarer.
        else
        {
            return Json(new { success = false, message = "No comments found" });
        }
    }
}

