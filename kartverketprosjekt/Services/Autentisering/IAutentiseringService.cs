using System.Security.Claims;

namespace kartverketprosjekt.Services.Autentisering
{
    public interface IAutentiseringService
    {
        public Task<(bool Success, string ErrorMessage, ClaimsPrincipal? Principal)> LoginAsync(string epost, string password);

        public bool IsHashedPassword(string password);

    }
}
