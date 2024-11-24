using kartverketprosjekt.Models;
using kartverketprosjekt.Services.Bruker;
using kartverketprosjekt.Services.Kommentar;
using kartverketprosjekt.Services.Sak;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

// ****************************************************************************************************************************
// ***********SaksbehandlerController er en controller som håndterer alle funksjoner for en saksbehandler og admin.************
// ****************************************************************************************************************************

namespace kartverketprosjekt.Controllers;

[Authorize(Roles = "3, 4")]
public class SaksbehandlerController : Controller
{
    private readonly ISakService _sakService;
    private readonly IKommentarService _kommentarService;
    private readonly IBrukerService _brukerService;

    public SaksbehandlerController(ISakService sakService, IKommentarService kommentarService, IBrukerService brukerService)
    {
        _sakService = sakService;
        _kommentarService = kommentarService;
        _brukerService = brukerService;
    }

    public async Task<IActionResult> CaseWorkerView()
{
    var brukerEpost = User.Identity.Name;
    var user = await _brukerService.GetUserByEmailAsync(brukerEpost);

    if (user == null || user.tilgangsnivaa_id < 3)
        return Forbid();

    ViewData["Saker"] =  _sakService.GetAllSaker();
    ViewBag.Saksbehandlere = await _brukerService.GetSaksbehandlereAsync();
    return View();
}

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status)
    {
        try
        {
            await _sakService.UpdateStatus(id, status);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(int sakID, string kommentar)
    {
        try
        {
            var brukerEpost = User.Identity.Name;
            await _kommentarService.AddComment(sakID, kommentar, brukerEpost);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetComments(int sakId)
    {
        // Fetch comments dynamically based on the sakId
        var kommentarer = await _kommentarService.GetComments(sakId);

        // Check if there are comments for the sakId
        if (kommentarer != null && kommentarer.Any())
        {
            return Json(new { success = true, kommentarer = kommentarer });
        }
        else
        {
            // Return a success response with an empty array to avoid previous comments showing up
            return Json(new { success = true, kommentarer = new List<KommentarModel>() });
        }
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _sakService.DeleteCase(id);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EndreSaksbehandler(int sakId, string saksbehandlerEpost)
    {
        try
        {
            await _sakService.AssignSaksbehandler(sakId, saksbehandlerEpost);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}