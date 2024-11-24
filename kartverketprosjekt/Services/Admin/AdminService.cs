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

        /// <summary>
        /// Henter statistikk for admin visning.
        /// </summary>
        /// <returns>En AdminStats-objekt med statistikkdata.</returns>
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

        /// <summary>
        /// Oppretter en ny bruker.
        /// </summary>
        /// <param name="epost">E-postadressen til brukeren.</param>
        /// <param name="passord">Passordet til brukeren.</param>
        /// <param name="tilgangsnivaa">Tilgangsnivået til brukeren.</param>
        /// <param name="organisasjon">Organisasjonen brukeren tilhører.</param>
        /// <param name="navn">Navnet til brukeren.</param>
        /// <returns>En tuple som indikerer suksess og en melding.</returns>
        public async Task<(bool Success, string Message)> CreateUserAsync(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn)
        {
            var existingUser = await _brukerRepository.GetUserByEmailAsync(epost);
            if (existingUser != null)
            {
                return (false, "En bruker med denne e-posten eksisterer allerede.");
            }

            var newUser = new BrukerModel
            {
                epost = epost,
                navn = navn,
                organisasjon = organisasjon,
                tilgangsnivaa_id = tilgangsnivaa,
                passord = _passwordHasher.HashPassword(null, passord)
            };

            await _brukerRepository.AddUserAsync(newUser);
            return (true, $"Brukeren {epost} ble opprettet.");
        }

        /// <summary>
        /// Oppdaterer tilgangsnivået til en bruker.
        /// </summary>
        /// <param name="userId">ID-en til brukeren.</param>
        /// <param name="newAccessLevel">Det nye tilgangsnivået.</param>
        /// <returns>En tuple som indikerer suksess og en melding.</returns>
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

        /// <summary>
        /// Sletter en bruker.
        /// </summary>
        /// <param name="email">E-postadressen til brukeren som skal slettes.</param>
        /// <param name="loggedInUserEmail">E-postadressen til den innloggede brukeren.</param>
        /// <returns>En tuple som indikerer suksess og en melding.</returns>
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

