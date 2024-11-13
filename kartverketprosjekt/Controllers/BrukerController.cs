using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using kartverketprosjekt.Services;
// ***********************************************************************************************************************
// ******BrukerController er en controller som håndterer alle funksjoner som kun skal være tilgjengelig for brukere.******
// ***********************************************************************************************************************

public class BrukerController : Controller
{
    
    private readonly IAutentiseringService _autentiseringService;
    private readonly IBrukerService _brukerService;
    private readonly INotifikasjonService _notifikasjonService;
    private readonly ISakService _sakService;


    public BrukerController(IAutentiseringService autentiseringService, IBrukerService brukerService, INotifikasjonService notifikasjonService, ISakService sakService)
    {
        _autentiseringService = autentiseringService;
        _brukerService = brukerService;
        _notifikasjonService = notifikasjonService;
        _sakService = sakService;
    }

    // Viser min side.
    public async Task<IActionResult> MyPage()
    {
        var bruker = User.Identity.Name;

        if (string.IsNullOrEmpty(bruker))
        {
            return RedirectToAction("Index", "Home");
        }

        // Use SakService to retrieve the cases
        var saker = await _sakService.GetUserCasesAsync(bruker);

        return View(saker);
    }

    // POST login
    // Metode for å logge inn en bruker.
    [ValidateAntiForgeryToken]
    [HttpPost]
    public async Task<IActionResult> Login(string epost, string password)
    {
        var (success, errorMessage, principal) = await _autentiseringService.LoginAsync(epost, password);
        if (!success)
        {
            TempData["ErrorMessage"] = errorMessage;
            return RedirectToAction("Index", "Home");
        }

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);
        return RedirectToAction("RegisterAreaChange", "Sak");
    }

    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        TempData["Message"] = "You have successfully logged out.";
        return RedirectToAction("Index", "Home");
    }


    // Metode for å registrere en ny bruker.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegistrerModel model)
    {
        if (!ModelState.IsValid)
        {
            TempData["ErrorMessage"] = "En feil oppstod under registreringen.";
            return RedirectToAction("Index", "Home");
        }

        var result = await _brukerService.RegisterUserAsync(model);

        if (result)
        {
            TempData["Message"] = $"Hei {model.navn}, registreringen er vellykket!";
            return RedirectToAction("RegisterAreaChange", "Sak");
        }

        TempData["ErrorMessage"] = "En feil oppstod under registreringen.";
        return RedirectToAction("Index", "Home");
    }


    // Metode for å oppdatere passordet til en bruker / Endre passord.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePassword(string password)
    {
        var result = await _brukerService.UpdatePasswordAsync(password);

        if (result)
        {
            TempData["SuccessMessage"] = "Passordet er oppdatert.";
        }
        else
        {
            TempData["ErrorMessage"] = "Kunne ikke oppdatere passordet.";
        }

        return RedirectToAction("Mypage");
    }

    // Metode for å oppdatere navnet til en bruker.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateName(string navn)
    {
        var result = await _brukerService.UpdateNameAsync(navn);

        if (result)
        {
            TempData["SuccessMessage"] = "Navnet er oppdatert.";
        }
        else
        {
            TempData["ErrorMessage"] = "Kunne ikke oppdatere navnet.";
        }

        return RedirectToAction("Mypage");
    }

    //DENNE METODEN BLIR ALDRI BRUKT. HVIS DEN SKAL BRUKES MÅ DEN OPPDATERES TIL Å BRUKE BRUKERSERVICE

    // Metode for å slette en bruker.     
    //[HttpPost]
    //public async Task<IActionResult> DeleteUser(string passord)
    //{

    //    // Prøver å slette brukeren.
    //    try
    //    {
    //        // Hent e-post fra den påloggede brukeren.
    //        string epost = User.Identity.Name;

    //        // Finn brukeren i databasen.
    //        var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
    //        if (bruker == null)
    //        {
    //            TempData["ErrorMessage"] = "Bruker finnes ikke.";
    //            Console.WriteLine("User not found"); // DEBUG
    //            return RedirectToAction("Mypage");
    //        }

    //        // Sjekk om passordet er korrekt
    //        var passwordHasher = new PasswordHasher<BrukerModel>();
    //        bool isPasswordValid;

    //        if (IsHashedPassword(bruker.passord))
    //        {
    //            // Verfier det hashede passordet.
    //            var result = passwordHasher.VerifyHashedPassword(bruker, bruker.passord, passord);
    //            isPasswordValid = result == PasswordVerificationResult.Success;
    //        }
    //        else
    //        {
    //            // Samenlikner klartekst passord.
    //            isPasswordValid = bruker.passord == passord;
    //        }

    //        // Hvis passordet er feil, vis feilmelding.
    //        if (!isPasswordValid)
    //        {
    //            TempData["ErrorMessage"] = "Feil passord. Vennligst prøv igjen.";
    //            Console.WriteLine("Invalid password");
    //            return RedirectToAction("Mypage");
    //        }

    //        // Slett brukeren
    //        _context.Bruker.Remove(bruker);
    //        await _context.SaveChangesAsync();
    //        Console.WriteLine("User deleted successfully");
    //        // Logg ut brukeren.
    //        await HttpContext.SignOutAsync();

    //        TempData["SuccessMessage"] = "Brukeren er slettet.";
    //        return RedirectToAction("Index", "Home"); // Redirect til ønsket side etter sletting
    //    }
    //    // Håndter eventuelle feil.
    //    catch (Exception ex)
    //    {
    //        Console.WriteLine($"Error deleting user: {ex.Message}");
    //        TempData["ErrorMessage"] = "Det oppstod en feil ved sletting av brukeren.";
    //        return RedirectToAction("Mypage");
    //    }
    //}

    // Metode for å slette en sak.
    [HttpPost]
    public async Task<IActionResult> DeleteCase(int id)
    {
        var isDeleted = await _sakService.DeleteCaseAsync(id);

        if (isDeleted)
        {
            TempData["SuccessMessage"] = $"Sak: {id} ble slettet";
        }
        else
        {
            TempData["ErrorMessage"] = "Kunne ikke slette sak";
        }

        return RedirectToAction("Mypage");
    }

    // Metode for å sjekke om en saksbehandler har endret status på saken sin
    [HttpGet]
    public async Task<IActionResult> HarEndretStatus()
    {
        bool hasNotification = await _notifikasjonService.HarEndretStatus(User.Identity.Name);
        return Json(hasNotification);
    }


    // Metode for å nullstille status_endret for alle saker som er relatert til brukeren
    [HttpPost]
    public async Task<IActionResult> ResetNotificationStatus()
    {
        bool success = await _notifikasjonService.ResetNotificationStatus(User.Identity.Name);
        return Json(new { success });
    }

    // Metode for å hente kommentarer til en sak.
    // Spesifik routing for å hente kommentarer til en sak.
    [Route("Bruker/GetComments")]
    [HttpGet]
    public async Task<IActionResult> GetComments(int sakId)
    {
        var kommentarer = await _sakService.GetCommentsAsync(sakId);

        if (kommentarer != null && kommentarer.Any())
        {
            return Json(new { success = true, kommentarer = kommentarer });
        }
        else
        {
            return Json(new { success = false, message = "No comments found" });
        }
    }
}

