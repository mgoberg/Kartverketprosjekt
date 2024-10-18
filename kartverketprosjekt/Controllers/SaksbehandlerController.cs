using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using Microsoft.EntityFrameworkCore;
using System.IO.Compression;

namespace kartverketprosjekt.Controllers;

public class SaksbehandlerController : Controller
{
    private readonly KartverketDbContext _context;

    public SaksbehandlerController(KartverketDbContext context)
    {
        _context = context;
    }

    public IActionResult CaseWorkerView()
    {
        // Henter alle saker fra databasen
        var saker = _context.Sak.ToList();

        // Oppretter en ViewBag eller ViewData for å sende data til visningen
        ViewData["Saker"] = saker;
        return View();
    }
    [HttpPost]
    public IActionResult AddComment(int sakID, string kommentar)
    {
        // Validering av input
        if (sakID <= 0 || string.IsNullOrWhiteSpace(kommentar))
        {
            return Json(new { success = false, message = "Ugyldig sakID eller kommentar." });
        }

        var nyKommentar = new KommentarModel()
        {
            SakID = sakID,
            Tekst = kommentar,
            Dato = DateTime.Now
        };

        _context.Kommentar.Add(nyKommentar);
        _context.SaveChanges();

        return Json(new { success = true });
    }
    [HttpGet]
    public JsonResult GetComments(int sakId)
    {
        // Hent kommentarer fra databasen basert på sakId
        var kommentarer = _context.Kommentar.Where(k => k.SakID == sakId).ToList();

        // Returner som JSON
        return Json(new { success = true, kommentarer });
    }

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