using Microsoft.AspNetCore.Mvc;
using kartverketprosjekt.Models;
using kartverketprosjekt.Services.Sak;

// ****************************************************************************************************************************
// ***********SakController er en controller som håndterer alle funksjoner i forbindelse med opprettelse av en sak.***********
// ****************************************************************************************************************************

namespace kartverketprosjekt.Controllers
{
    public class SakController : Controller
    {
        private readonly ISakService _sakService;

        public SakController(ISakService sakService)
        {
            _sakService = sakService;
        }

        [HttpGet]
        public async Task<IActionResult> RegisterAreaChange()
        {
            ViewBag.Saksbehandlere = await _sakService.GetCaseworkersAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterAreaChange(SakModel sak, IFormFile vedlegg, double nord, double ost, int koordsys)
        {
            var currentUserEmail = User.Identity?.Name;
            var sakId = await _sakService.RegisterCaseAsync(sak, vedlegg, nord, ost, koordsys, currentUserEmail);
            TempData["id"] = sakId;
            return RedirectToAction("AreaChangeOverview");
        }

        [HttpGet]
        public IActionResult AreaChangeOverview()
        {
            if (TempData.ContainsKey("id"))
            {
                int id = (int)TempData["id"];
                var sak = _sakService.GetCaseById(id);

                if (sak == null)
                {
                    return NotFound();
                }

                return View(sak);
            }

            return NotFound();
        }
    }


}
