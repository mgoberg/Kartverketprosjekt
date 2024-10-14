using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
            var result = passwordHasher.VerifyHashedPassword(user, user.passord, password);

            if (result == PasswordVerificationResult.Success)
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
            var passwordHasher = new PasswordHasher<BrukerModel>();

            // Hash the password and create a new user
            var bruker = new BrukerModel
            {
                passord = passwordHasher.HashPassword(null, model.passord),
                epost = model.epost,
                tilgangsnivaa_id = 1 // Set appropriate access level
            };

            // Add user to the database
            await _context.AddAsync(bruker);
            await _context.SaveChangesAsync();

            // Automatically log in the user after registration
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, bruker.epost),
            new Claim(ClaimTypes.Role, bruker.tilgangsnivaa_id.ToString()) // Optional: Handle roles
        };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            TempData["Message"] = "Registration successful! You are now logged in.";
            return RedirectToAction("RegisterAreaChange", "Sak");
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
        return BadRequest(new { success = false, errors });
    }
}
