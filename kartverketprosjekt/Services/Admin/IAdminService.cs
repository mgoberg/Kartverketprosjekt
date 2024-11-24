using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services.Admin
{
    public interface IAdminService
    {
        Task<AdminStats> GetAdminViewStatsAsync();
        Task<(bool Success, string Message)> UpdateUserAccessAsync(string userId, int newAccessLevel);
        Task<(bool Success, string Message)> DeleteUserAsync(string email, string loggedInUserEmail);
        Task<(bool Success, string Message)> CreateUserAsync(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn);
        


    }
}
