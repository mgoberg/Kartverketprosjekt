using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using System.Security.Cryptography;
using Microsoft.EntityFrameworkCore;

public class BrukerController : Controller
{
    private readonly KartverketDbContext _context;

    public BrukerController(KartverketDbContext context)
    {
        _context = context;
    }

    // POST login
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Login(string epost, string password)
    {
        // Retrieve the user from the database
        var user = _context.Bruker.FirstOrDefault(u => u.epost == epost);

        if (user != null)
        {
            var passwordHasher = new PasswordHasher<BrukerModel>();

            // Check if the password in the database is hashed or not
            bool isPasswordValid;

            if (IsHashedPassword(user.passord))
            {
                // Verify hashed password
                var result = passwordHasher.VerifyHashedPassword(user, user.passord, password);
                isPasswordValid = result == PasswordVerificationResult.Success;
            }
            else
            {
                // Compare plaintext password
                isPasswordValid = user.passord == password;
            }

            if (isPasswordValid)
            {
                // Create claims for the logged-in user
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.epost),
                new Claim(ClaimTypes.Role, user.tilgangsnivaa_id.ToString()) // Optional: Handle roles
            };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                return RedirectToAction("RegisterAreaChange", "Sak");
            }
        }

        // If user is not found or password is incorrect, show an error message
        ViewBag.ErrorMessage = "Invalid login attempt";
        return View("Index");
    }

    private bool IsHashedPassword(string password)
    {
        // Example: Assume that a hashed password is at least 60 characters (bcrypt or similar)
        return password.Length >= 60;
    }

    // POST logout
    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Set a success message for the logout
        TempData["Message"] = "You have successfully logged out.";
        return RedirectToAction("Index", "Home"); // Redirect to the desired page after logout
    }

    // POST register
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
                tilgangsnivaa_id = 1 // Set appropriate access level
            };

            // Nullstiller passord før lagring slik at passordet ikke finnes i klartekst.
            model.passord = null;

            // Add user to the database
            await _context.AddAsync(bruker);
            await _context.SaveChangesAsync();

            // Logger inn bruker etter registrering
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
        TempData["ErrorMessage"] = "En feil oppstod under registreringen.";
        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePassword(string password)
    {
        // Assume the user's email is retrieved from the session or identity information.
        string epost = User.Identity.Name; // Adjust this as needed based on your authentication setup

        var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
        if (bruker != null)
        {
            var passwordHasher = new PasswordHasher<BrukerModel>();
            bruker.passord = passwordHasher.HashPassword(bruker, password); // Hash the new password

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Passordet er oppdatert.";
            return RedirectToAction("Mypage", "Home"); // Redirect to /Home/Mypage
        }

        TempData["ErrorMessage"] = "Kunne ikke oppdatere passordet.";
        return RedirectToAction("Mypage", "Home"); // Redirect to /Home/Mypage with an error message
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateName(string navn)
    {
        // Assume the user's email is retrieved from the session or identity information.
        string epost = User.Identity.Name; // Adjust this as needed based on your authentication setup

        var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
        if (bruker != null)
        {
            bruker.navn = navn; // Update the user's name

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Navnet er oppdatert.";
            return RedirectToAction("Mypage", "Home"); // Redirect to /Home/Mypage
        }

        TempData["ErrorMessage"] = "Kunne ikke oppdatere navnet.";
        return RedirectToAction("Mypage", "Home"); // Redirect to /Home/Mypage with an error message
    }

    // TODO: Her må vi gjøre slik at passordet må kunne skrives uten hash for å bekrefte sletting
    [HttpPost]
    public async Task<IActionResult> DeleteUser(string passord)
    {
        try
        {
            Console.WriteLine($"Received password: {passord}");

            // Hent e-post fra den påloggede brukeren
            string epost = User.Identity.Name;

            // Finn brukeren i databasen
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
            if (bruker == null)
            {
                TempData["ErrorMessage"] = "Bruker finnes ikke.";
                Console.WriteLine("User not found");
                return RedirectToAction("Mypage", "Home");
            }



            // Sjekk om passordet er korrekt
            var passwordHasher = new PasswordHasher<BrukerModel>();
            bool isPasswordValid;

            if (IsHashedPassword(bruker.passord))
            {
                // Verify hashed password
                var result = passwordHasher.VerifyHashedPassword(bruker, bruker.passord, passord);
                isPasswordValid = result == PasswordVerificationResult.Success;
            }
            else
            {
                // Compare plaintext password
                isPasswordValid = bruker.passord == passord;
            }

            if (!isPasswordValid)
            {
                TempData["ErrorMessage"] = "Feil passord. Vennligst prøv igjen.";
                Console.WriteLine("Invalid password");
                return RedirectToAction("Mypage", "Home");
            }

            // Slett brukeren
            _context.Bruker.Remove(bruker);
            await _context.SaveChangesAsync();
            Console.WriteLine("User deleted successfully");
            await HttpContext.SignOutAsync();

            TempData["SuccessMessage"] = "Brukeren er slettet.";
            return RedirectToAction("Index", "Home"); // Redirect til ønsket side etter sletting
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error deleting user: {ex.Message}");
            TempData["ErrorMessage"] = "Det oppstod en feil ved sletting av brukeren.";
            return RedirectToAction("Mypage", "Home");
        }
    }

}

