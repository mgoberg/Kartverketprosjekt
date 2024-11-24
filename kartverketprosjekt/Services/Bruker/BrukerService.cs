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

        /// <summary>
        /// Registrerer en ny bruker.
        /// </summary>
        /// <param name="model">Modellen som inneholder registreringsinformasjon.</param>
        /// <returns>True hvis registreringen var vellykket, ellers false.</returns>
        public async Task<bool> RegisterUserAsync(RegistrerModel model)
        {
            if (model == null) return false;

            var existingUser = await _repository.GetUserByEmailAsync(model.epost);
            if (existingUser != null)
            {
                return false;
            }

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

        /// <summary>
        /// Oppdaterer navnet til den innloggede brukeren.
        /// </summary>
        /// <param name="navn">Det nye navnet.</param>
        /// <returns>True hvis oppdateringen var vellykket, ellers false.</returns>
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

        /// <summary>
        /// Oppdaterer passordet til den innloggede brukeren.
        /// </summary>
        /// <param name="password">Det nye passordet.</param>
        /// <returns>True hvis oppdateringen var vellykket, ellers false.</returns>
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

        /// <summary>
        /// Henter en bruker basert på e-postadressen.
        /// </summary>
        /// <param name="email">E-postadressen til brukeren.</param>
        /// <returns>Brukerobjektet hvis det finnes, ellers null.</returns>
        public async Task<BrukerModel> GetUserByEmailAsync(string email)
        {
            return await _repository.GetUserByEmailAsync(email);
        }

        /// <summary>
        /// Henter alle saksbehandlere.
        /// </summary>
        /// <returns>En liste over saksbehandlere.</returns>
        public async Task<List<BrukerModel>> GetSaksbehandlereAsync()
        {
            
            const int saksbehandlerRole = 3;

            return await _repository.GetUsersByAccessLevelAsync(saksbehandlerRole);
        }
    }
}
