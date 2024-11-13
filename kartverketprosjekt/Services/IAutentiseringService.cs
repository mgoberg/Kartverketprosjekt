using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace kartverketprosjekt.Services
{
    public interface IAutentiseringService
    {
        public Task<(bool Success, string ErrorMessage, ClaimsPrincipal? Principal)> LoginAsync(string epost, string password);

        public bool IsHashedPassword(string password);

    }
}
