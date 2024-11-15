using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;

namespace kartverketprosjekt.Services.Bruker
{
    public class BrukerService : IBrukerService
    {
        private readonly KartverketDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<BrukerModel> _passwordHasher;

        public BrukerService(KartverketDbContext context, IHttpContextAccessor httpContextAccessor, IPasswordHasher<BrukerModel> passwordHasher)
        {
            _context = context;
            _httpContextAccessor = httpContextAccessor;
            _passwordHasher = passwordHasher;
        }
        public async Task<bool> RegisterUserAsync(RegistrerModel model)
        {
            if (model == null) return false;

            var bruker = new BrukerModel
            {
                passord = _passwordHasher.HashPassword(null, model.passord),
                epost = model.epost,
                navn = model.navn,
                tilgangsnivaa_id = 1
            };

            await _context.AddAsync(bruker);
            await _context.SaveChangesAsync();

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, bruker.epost),
                new Claim(ClaimTypes.Role, bruker.tilgangsnivaa_id.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            await _httpContextAccessor.HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return true;
        }
        public async Task<bool> UpdateNameAsync(string navn)
        {
            // Get the email of the logged-in user
            string epost = _httpContextAccessor.HttpContext.User.Identity.Name;

            // Find the user in the database
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);
            if (bruker != null)
            {
                bruker.navn = navn; // Update the user's name

                // Save changes to the database
                await _context.SaveChangesAsync();
                return true; // Indicate that the update was successful
            }

            return false; // Indicate that the update failed
        }
        public async Task<bool> UpdatePasswordAsync(string password)
        {
            // Retrieve the email of the logged-in user
            string epost = _httpContextAccessor.HttpContext.User.Identity.Name;

            // Find the user in the database
            var bruker = await _context.Bruker.FirstOrDefaultAsync(b => b.epost == epost);

            if (bruker != null)
            {
                // Hash the new password and update the user's password
                bruker.passord = _passwordHasher.HashPassword(bruker, password);

                // Save the changes to the database
                await _context.SaveChangesAsync();
                return true; // Indicate that the update was successful
            }

            return false; // Indicate that the update failed
        }
        public BrukerModel GetUserByEmail(string email)
        {
            return _context.Bruker.FirstOrDefault(b => b.epost == email);
        }

        public List<BrukerModel> GetSaksbehandlere()
        {
            return _context.Bruker.Where(b => b.tilgangsnivaa_id == 3 || b.tilgangsnivaa_id == 4).ToList();
        }
    }
}
