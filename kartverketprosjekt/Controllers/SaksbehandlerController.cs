using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;
using Microsoft.AspNetCore.Authorization;

// ****************************************************************************************************************************
// ***********SaksbehandlerController er en controller som håndterer alle funksjoner for en saksbehandler og admin.************
// ****************************************************************************************************************************

namespace kartverketprosjekt.Controllers;

// Gir tilgang til SaksbehandlerController kun for brukere med rolle 3 og 4 (saksbehandler og admin brukere)
[Authorize(Roles = "3, 4")]
public class SaksbehandlerController : Controller
{
    private readonly KartverketDbContext _context;

    public SaksbehandlerController(KartverketDbContext context)
    {
        _context = context;
    }
    private string GetAuthenticatedUserEmail()
    {
        return User.Identity.Name; // Henter brukerens e-post fra identiteten
    }

    // Metode for å vise saksbehandler viewet.
    public IActionResult CaseWorkerView()
    {
        // Hent den autentiserte brukerens e-post
        var brukerEpost = GetAuthenticatedUserEmail();

        // Sjekk om brukeren finnes i Bruker-tabellen
        var user = _context.Bruker.FirstOrDefault(u => u.epost == brukerEpost);

        // Sjekk om brukeren har tilgangsnivå 3 eller høyere
        if (user == null || user.tilgangsnivaa_id < 3)
        {
            // Returner en 403 Forbidden-statuskode hvis tilgangsnivået er lavere enn 3
            return Forbid();
        }

        // Henter alle saker fra databasen
        var saker = _context.Sak.ToList();

        // Oppretter en ViewBag eller ViewData for å sende data til visningen
        ViewData["Saker"] = saker;
        return View();
    }

    // Metode for å oppdatere status på en sak.
    [HttpPost]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        var sak = await _context.Sak.FindAsync(id); // Hent saken ved ID

        // Hvis saken ikke finnes i databasen returneres en feilmelding.
        if (sak == null)
        {
            return Json(new { success = false, message = "Sak ikke funnet." });
        }

        if (sak.status != status) // Sjekk om status er endret
        {
            sak.status = status; // Oppdater status
            sak.status_endret = true; // Marker at status er endret
            await _context.SaveChangesAsync(); // Lagre endringer i databasen

            return Json(new { success = true, message = "Status oppdatert og merket som endret." });
        }

        // Hvis status ikke er endret
        return Json(new { success = false, message = "Ny status er den samme som eksisterende status." });
    }

    // Metode for å legge til en kommentar på en sak.
    [HttpPost]
    public async Task<IActionResult> AddComment(int sakID, string kommentar, string epost)
    {
        var sak = await _context.Sak.FindAsync(sakID);
        // Validering av input
        if (sakID <= 0 || string.IsNullOrWhiteSpace(kommentar))
        {
            return Json(new { success = false, message = "Ugyldig sakID eller kommentar." });
        }

        // Hent e-post til den innloggede brukeren
        var brukerEpost = GetAuthenticatedUserEmail();

        // Sjekk om brukeren eksisterer i Bruker-tabellen
        var bruker = _context.Bruker.FirstOrDefault(b => b.epost == brukerEpost);
        if (bruker == null)
        {
            return Json(new { success = false, message = "Bruker ikke funnet." });
        }

        // Opprett en ny kommentar.
        var nyKommentar = new KommentarModel()
        {
            SakID = sakID,
            Tekst = kommentar,
            Dato = DateTime.Now,
            Epost = brukerEpost
        };

        // Endre status_endret til true for notifikasjon.
        sak.status_endret = true;

        // Legg til kommentaren i databasen.
        _context.Kommentar.Add(nyKommentar);
        _context.SaveChanges();

        return Json(new { success = true });
    }

    // Metode for å hent kommentarer for en sak.
    [HttpGet]
    public JsonResult GetComments(int sakId)
    {
        // Hent alle kommentarer for en sak.
        var kommentarer = _context.Kommentar
            .Where(k => k.SakID == sakId)
            .Select(k => new
            {
                k.Tekst,
                k.Dato,
                k.Epost 
            }).ToList();

        return Json(new { success = true, kommentarer });
    }

    // Metode for å slette en sak.
    [HttpPost]
    public IActionResult Delete(int id)
    {
        // Finn saken i databasen basert på ID
        var sak = _context.Sak.Include(s => s.Kommentarer).FirstOrDefault(s => s.id == id);

        if (sak != null)
        {
            // Fjern alle kommentarer tilhørende saken
            _context.Kommentar.RemoveRange(sak.Kommentarer);

            // Fjern saken fra databasen
            _context.Sak.Remove(sak);
            _context.SaveChanges();
            return Json(new { success = true, message = "Sak og tilhørende kommentarerer slettet." });
        }

        return Json(new { success = false, message = "Sak ikke funnet." });
    }


}