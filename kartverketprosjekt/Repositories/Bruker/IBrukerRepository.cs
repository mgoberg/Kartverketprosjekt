using kartverketprosjekt.Models;

namespace kartverketprosjekt.Repositories.Bruker
{
    public interface IBrukerRepository
    {
        Task<List<BrukerModel>> GetAllUsersAsync();
        Task<BrukerModel?> GetUserByIdAsync(string userId);
        Task<BrukerModel?> GetUserByEmailAsync(string email);
        Task<bool> DeleteUserAsync(BrukerModel user);
        Task<int> GetUserCountAsync();
        Task AddUserAsync(BrukerModel user);
        Task<List<BrukerModel>> GetUsersByAccessLevelAsync(int accessLevel);

        Task SaveChangesAsync();
    }
}
