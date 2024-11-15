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

    public IActionResult CaseWorkerView()
    {
        var brukerEpost = User.Identity.Name;
        var user = _brukerService.GetUserByEmail(brukerEpost);

        if (user == null || user.tilgangsnivaa_id < 3)
            return Forbid();

        ViewData["Saker"] = _sakService.GetAllSaker();
        ViewBag.Saksbehandlere = _brukerService.GetSaksbehandlere();
        return View();
    }

    [HttpPost]
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
    public JsonResult GetComments(int sakId)
    {
        var kommentarer = _kommentarService.GetComments(sakId);
        return Json(new { success = true, kommentarer });
    }

    [HttpPost]
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