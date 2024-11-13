using Microsoft.AspNetCore.Mvc;
using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using kartverketprosjekt.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;

// ****************************************************************************************************************************
// ***********SakController er en controller som håndterer alle funksjoner i forbeindelse med opprettelse av en sak.***********
// ****************************************************************************************************************************

namespace kartverketprosjekt.Controllers
{
    public class SakController : Controller
    {
        private readonly KartverketDbContext _context;

        private readonly DiscordBot _discordBot; // Discord bot for å sende notifikasjon


        private readonly IKommuneInfoService _kommuneInfoService;

        private readonly IStedsnavnService _stedsnavnService; // Kan fjernes hvis ikke vi skal implementere stedsnavn api

        // Konstruktør for SakController.
        public SakController(DiscordBot discordBot, KartverketDbContext context, ILogger<HomeController> logger, IKommuneInfoService kommuneInfoService, IStedsnavnService stedsnavnService)
        {
            _discordBot = discordBot; // Initialize the DiscordBot
            _context = context;
            _kommuneInfoService = kommuneInfoService;
            _stedsnavnService = stedsnavnService; //kan fjernes hvis ikke vi skal implementere stedsnavn api
        }




      
        // Metode for å vise registreringssiden for en sak.

        [HttpGet]
        public async Task<IActionResult> RegisterAreaChange()
        {
            // Load available caseworkers (brukere with tilgangsnivaa_id = 3, indicating Saksbehandler role)
            ViewBag.Saksbehandlere = new SelectList(await _context.Bruker
                .Where(b => b.tilgangsnivaa_id == 3) // Filter by Saksbehandler role
                .ToListAsync(), "epost", "navn");

            return View();
        }


        // Metode for å registrere en sak.

        [HttpPost]
        public async Task<IActionResult> RegisterAreaChange(SakModel sak, IFormFile vedlegg, double nord, double ost, int koordsys)
        {
            // Hent e-post fra den innloggede brukeren
            sak.epost_bruker = User.Identity.Name;
            
            // Hent brukerens tilgangsnivå fra databasen
            if(User.Identity.IsAuthenticated)
            {
                
                var bruker = await _context.Bruker
                    .FirstOrDefaultAsync(b => b.epost == sak.epost_bruker);
                sak.IsPriority = bruker.tilgangsnivaa_id == 2;
            }
            

            // Hardkodede verdier for kommune_id og status
            sak.kommune_id = 1;
            sak.status = "Ubehandlet";

            // Sjekk om vedlegg er lastet opp
            if (vedlegg != null && vedlegg.Length > 0)
            {
                // Definerer filsti for opplasting av vedlegg.
                var uploadsPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Sikrer at mappen eksisterer.
                if (!Directory.Exists(uploadsPath))
                {
                    Directory.CreateDirectory(uploadsPath);
                }

                // Genererer et unikt filnavn for vedlegget.
                var fileName = Guid.NewGuid().ToString() + Path.GetExtension(vedlegg.FileName);
                var filePath = Path.Combine(uploadsPath, fileName);

                // Lagrer vedlegget på "serveren".
                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await vedlegg.CopyToAsync(stream);
                }

                // Sender filnavnet til sak-objektet.
                sak.vedlegg = fileName;
            }

            // Gjør API-kall med nord og ost
            var kommuneInfo = await _kommuneInfoService.GetKommuneInfoAsync(nord, ost, koordsys);
            if (kommuneInfo != null)
            {   // API-responsen lagres i KommuneInfo modellen og så flytter vi det over til Sak modellen
                sak.Kommunenavn = kommuneInfo.Kommunenavn; 
                sak.Kommunenummer = kommuneInfo.Kommunenummer;
                sak.Fylkesnavn = kommuneInfo.Fylkesnavn;
                sak.Fylkesnummer = kommuneInfo.Fylkesnummer;
            }

            // Auto-assign the caseworker with the least number of cases
            var leastAssignedCaseworker = await _context.Bruker
                .Where(b => b.tilgangsnivaa_id == 3) // Only caseworkers
                .OrderBy(b => _context.Sak.Count(s => s.SaksbehandlerId == b.epost)) // Order by case count
                .FirstOrDefaultAsync();

            // Assign the caseworker with the fewest cases
            if (leastAssignedCaseworker != null)
            {
                sak.SaksbehandlerId = leastAssignedCaseworker.epost;
            }

            // Lagre saken i databasen
            _context.Sak.Add(sak);
            await _context.SaveChangesAsync();

            TempData["id"] = sak.id;
            string roleID = "1300573258919182347"; // Role ID for the Discord channel

            //Sender melding i discord kanal på ny sak
            await _discordBot.SendMessageToDiscord($"**En ny sak er opprettet i {sak.Kommunenavn}**\n**Beskrivelse:** {sak.beskrivelse}\n**Opprettet av:** {sak.epost_bruker}\n<@&{roleID}> " );

            // Viderefør til oversiktsiden
            return RedirectToAction("AreaChangeOverview");
        }

        // Metode for å vise kvitteringen på en sak.
        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            // Hvis TempData inneholder id, hent saken basert på id.
            if (TempData.ContainsKey("id"))
            {
                int id = (int)TempData["id"];

                // Hent den spesifikke saken basert på id
                var sak = _context.Sak.FirstOrDefault(s => s.id == id);

                if (sak == null)
                {
                    return NotFound(); // Håndterer hvis saken ikke finnes
                }

                return View(sak); // Sender den innsendte saken til viewet
            }

            return NotFound(); // Handle når id ikke er i TempData.
        }

    }

}
