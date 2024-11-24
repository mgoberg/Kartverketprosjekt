using kartverketprosjekt.Models;

namespace kartverketprosjekt.Services.Bruker
{
    public interface IBrukerService
    {
        public Task<bool> RegisterUserAsync(RegistrerModel model);
        public Task<bool> UpdateNameAsync(string navn);
        public Task<bool> UpdatePasswordAsync(string password);
        public Task<BrukerModel> GetUserByEmailAsync(string email);
        public Task<List<BrukerModel>> GetSaksbehandlereAsync();
    }
}
