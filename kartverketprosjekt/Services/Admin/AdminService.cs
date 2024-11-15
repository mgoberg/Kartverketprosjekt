using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Identity;

namespace kartverketprosjekt.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly KartverketDbContext _context;

        public AdminService(KartverketDbContext context)
        {
            _context = context;
        }

        public AdminStats GetAdminViewStats()
        {
            return new AdminStats
            {
                CaseCount = _context.Sak.Count(),
                UserCount = _context.Bruker.Count(),
                OpenCasesUnbehandlet = _context.Sak.Count(s => s.status == "Ubehandlet"),
                OpenCasesUnderBehandling = _context.Sak.Count(s => s.status == "Under Behandling"),
                OpenCasesAvvist = _context.Sak.Count(s => s.status == "Avvist"),
                OpenCasesArkivert = _context.Sak.Count(s => s.status == "Arkivert"),
                ClosedCases = _context.Sak.Count(s => s.status == "Løst"),
                Users = _context.Bruker.ToList()
            };
        }
        public bool UpdateUserAccess(string userId, int newAccessLevel, out string message)
        {
            var user = _context.Bruker.Find(userId);
            if (user != null)
            {
                user.tilgangsnivaa_id = newAccessLevel;
                _context.SaveChanges();
                message = $"Endret tilgangsnivå for {userId} til: {newAccessLevel}";
                return true;
            }

            message = "Bruker ikke funnet.";
            return false;
        }
        public bool DeleteUser(string email, string loggedInUserEmail, out string errorMessage)
        {
            if (string.IsNullOrEmpty(email))
            {
                errorMessage = "E-post må være oppgitt.";
                return false;
            }

            var user = _context.Bruker.FirstOrDefault(u => u.epost == email);
            if (user == null)
            {
                errorMessage = "Bruker ikke funnet.";
                return false;
            }

            if (user.epost == loggedInUserEmail)
            {
                errorMessage = "Du kan ikke slette din egen konto.";
                return false;
            }

            var linkedCases = _context.Sak.Any(s => s.epost_bruker == user.epost);
            if (linkedCases)
            {
                errorMessage = "Brukeren kan ikke slettes fordi det finnes saker knyttet til denne brukeren.";
                return false;
            }

            _context.Bruker.Remove(user);
            _context.SaveChanges();
            errorMessage = $"{user.epost} ble slettet.";
            return true;
        }
        public bool CreateUser(string email, string password, int accessLevel, string organization, string? name, out string errorMessage)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(organization))
            {
                errorMessage = "E-post, passord og organisasjon må fylles ut.";
                return false;
            }

            if (_context.Bruker.Any(b => b.epost == email))
            {
                errorMessage = "En bruker med denne e-posten eksisterer allerede.";
                return false;
            }

            var passwordHasher = new PasswordHasher<BrukerModel>();
            var newUser = new BrukerModel
            {
                epost = email,
                passord = passwordHasher.HashPassword(null, password),
                tilgangsnivaa_id = accessLevel,
                organisasjon = organization,
                navn = name
            };

            _context.Bruker.Add(newUser);
            _context.SaveChanges();
            errorMessage = $"Bruker med e-post {email} ble opprettet.";
            return true;
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
