using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using kartverketprosjekt.Repositories.Bruker;
using kartverketprosjekt.Repositories.Sak;
using Microsoft.AspNetCore.Identity;

namespace kartverketprosjekt.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IBrukerRepository _brukerRepository;
        private readonly ISakRepository _sakRepository;
        private readonly PasswordHasher<BrukerModel> _passwordHasher;

        public AdminService(IBrukerRepository brukerRepository, ISakRepository sakRepository)
        {
            _brukerRepository = brukerRepository;
            _sakRepository = sakRepository;
            _passwordHasher = new PasswordHasher<BrukerModel>();
        }

        public async Task<AdminStats> GetAdminViewStatsAsync()
        {
            var users = await _brukerRepository.GetAllUsersAsync();

            return new AdminStats
            {
                CaseCount = await _sakRepository.GetCaseCountAsync(),
                UserCount = await _brukerRepository.GetUserCountAsync(),
                OpenCasesUnbehandlet = await _sakRepository.GetCaseCountByStatusAsync("Ubehandlet"),
                OpenCasesUnderBehandling = await _sakRepository.GetCaseCountByStatusAsync("Under Behandling"),
                OpenCasesAvvist = await _sakRepository.GetCaseCountByStatusAsync("Avvist"),
                OpenCasesArkivert = await _sakRepository.GetCaseCountByStatusAsync("Arkivert"),
                ClosedCases = await _sakRepository.GetCaseCountByStatusAsync("Løst"),
                Users = users
            };
        }
        public async Task<(bool Success, string Message)> CreateUserAsync(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn)
        {
            // Check if the email is already registered
            var existingUser = await _brukerRepository.GetUserByEmailAsync(epost);
            if (existingUser != null)
            {
                return (false, "En bruker med denne e-posten eksisterer allerede.");
            }

            // Create a new user model
            var newUser = new BrukerModel
            {
                epost = epost,
                navn = navn,
                organisasjon = organisasjon,
                tilgangsnivaa_id = tilgangsnivaa,
                passord = _passwordHasher.HashPassword(null, passord) // Hash the password
            };

            // Save the new user
            await _brukerRepository.AddUserAsync(newUser);
            return (true, $"Brukeren {epost} ble opprettet.");
        }
        public async Task<(bool Success, string Message)> UpdateUserAccessAsync(string userId, int newAccessLevel)
        {
            var user = await _brukerRepository.GetUserByIdAsync(userId);
            if (user != null)
            {
                user.tilgangsnivaa_id = newAccessLevel;
                await _brukerRepository.SaveChangesAsync();
                return (true, $"Endret tilgangsnivå for {userId} til: {newAccessLevel}");
            }

            return (false, "Bruker ikke funnet.");
        }

        public async Task<(bool Success, string Message)> DeleteUserAsync(string email, string loggedInUserEmail)
        {
            var user = await _brukerRepository.GetUserByEmailAsync(email);
            if (user == null)
            {
                return (false, "Bruker ikke funnet.");
            }

            if (user.epost == loggedInUserEmail)
            {
                return (false, "Du kan ikke slette din egen konto.");
            }

            var linkedCases = (await _sakRepository.GetAllCasesAsync()).Any(s => s.epost_bruker == user.epost);
            if (linkedCases)
            {
                return (false, "Brukeren kan ikke slettes fordi det finnes saker knyttet til denne brukeren.");
            }

            await _brukerRepository.DeleteUserAsync(user);
            return (true, $"{user.epost} ble slettet.");
        }
    }

    public class AdminStats
    {
        public int CaseCount { get; set; }
        public int UserCount { get; set; }
        public int OpenCasesUnbehandlet { get; set; }
        public int OpenCasesUnderBehandling { get; set; }
        public int OpenCasesAvvist { get; set; }
        public int OpenCasesArkivert { get; set; }
        public int ClosedCases { get; set; }
        public List<BrukerModel> Users { get; set; }
    }
}

