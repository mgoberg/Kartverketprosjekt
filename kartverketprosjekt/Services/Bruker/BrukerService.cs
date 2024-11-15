using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using kartverketprosjekt.Repositories.Bruker;

namespace kartverketprosjekt.Services.Bruker
{
    public class BrukerService : IBrukerService
    {
        private readonly IBrukerRepository _repository;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPasswordHasher<BrukerModel> _passwordHasher;

        public BrukerService(IBrukerRepository repository, IHttpContextAccessor httpContextAccessor, IPasswordHasher<BrukerModel> passwordHasher)
        {
            _repository = repository;
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

            await _repository.AddUserAsync(bruker);
            await _repository.SaveChangesAsync();

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
            string epost = _httpContextAccessor.HttpContext.User.Identity.Name;
            var bruker = await _repository.GetUserByEmailAsync(epost);

            if (bruker != null)
            {
                bruker.navn = navn;
                await _repository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<bool> UpdatePasswordAsync(string password)
        {
            string epost = _httpContextAccessor.HttpContext.User.Identity.Name;
            var bruker = await _repository.GetUserByEmailAsync(epost);

            if (bruker != null)
            {
                bruker.passord = _passwordHasher.HashPassword(bruker, password);
                await _repository.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<BrukerModel> GetUserByEmailAsync(string email)
        {
            return await _repository.GetUserByEmailAsync(email);
        }

        public async Task<List<BrukerModel>> GetSaksbehandlereAsync()
        {
            // Assume "3" is the tilgangsnivaa_id for saksbehandlere
            const int saksbehandlerRole = 3;

            return await _repository.GetUsersByAccessLevelAsync(saksbehandlerRole);
        }

    }
}
