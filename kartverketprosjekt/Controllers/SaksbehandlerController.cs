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
}