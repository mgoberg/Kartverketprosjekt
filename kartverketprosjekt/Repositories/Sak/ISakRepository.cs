using kartverketprosjekt.Models;

namespace kartverketprosjekt.Repositories.Sak
{
    public interface ISakRepository
    {
        Task<int> GetCaseCountAsync();
        Task<int> GetCaseCountByStatusAsync(string status);
        Task<List<SakModel>> GetAllCasesAsync();
        Task SaveChangesAsync();
    }
}

