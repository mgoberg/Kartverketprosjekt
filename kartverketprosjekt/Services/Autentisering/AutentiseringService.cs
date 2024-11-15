using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using kartverketprosjekt.Data;

namespace kartverketprosjekt.Services.Autentisering
{
    public class AutentiseringService : IAutentiseringService
    {
        private readonly KartverketDbContext _context;
        private readonly PasswordHasher<BrukerModel> _passwordHasher;

        public AutentiseringService(KartverketDbContext context, PasswordHasher<BrukerModel> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public async Task<(bool Success, string ErrorMessage, ClaimsPrincipal? Principal)> LoginAsync(string epost, string password)
        {
            var user = await _context.Bruker.FirstOrDefaultAsync(u => u.epost == epost);
            if (user == null)
                return (false, "Feil epost eller passord", null);

            bool isPasswordValid = IsHashedPassword(user.passord)
                ? _passwordHasher.VerifyHashedPassword(user, user.passord, password) == PasswordVerificationResult.Success
                : user.passord == password;

            if (!isPasswordValid)
                return (false, "Feil epost eller passord", null);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.epost),
                new Claim(ClaimTypes.Role, user.tilgangsnivaa_id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(claimsIdentity);

            return (true, "", principal);
        }

        public bool IsHashedPassword(string password) => password.Length >= 60;
    }


}
