using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using kartverketprosjekt.Services.Admin;
using System.Threading.Tasks;
using kartverketprosjekt.Models;

namespace kartverketprosjekt.Controllers

// ****************************************************************************************************************************
// ******AdminController er en controller som håndterer alle funksjoner som kun skal være tilgjengelig for Administrator.******
// ****************************************************************************************************************************
{
    [Authorize(Roles = "4")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        public async Task<IActionResult> AdminView()
        {
            var stats = await _adminService.GetAdminViewStatsAsync();
            var viewModel = new AdminPageViewModel
            {
                CaseCount = stats.CaseCount,
                UserCount = stats.UserCount,
                OpenCasesUnbehandlet = stats.OpenCasesUnbehandlet,
                OpenCasesUnderBehandling = stats.OpenCasesUnderBehandling,
                OpenCasesAvvist = stats.OpenCasesAvvist,
                OpenCasesArkivert = stats.OpenCasesArkivert,
                ClosedCases = stats.ClosedCases,
                Users = stats.Users
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAccess(string userId, int newAccessLevel)
        {
            var (success, message) = await _adminService.UpdateUserAccessAsync(userId, newAccessLevel);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("AdminView");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SlettBruker(string epost)
        {
            var loggedInUserEmail = User.Identity.Name;
            var (success, errorMessage) = await _adminService.DeleteUserAsync(epost, loggedInUserEmail);

            if (success)
            {
                TempData["SuccessMessage"] = errorMessage;
                return RedirectToAction("AdminView");
            }

            ViewBag.ErrorMessage = errorMessage;
            var stats = await _adminService.GetAdminViewStatsAsync();
            return View("AdminView", stats.Users);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OpprettBruker(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn)
        {
            var (success, message) = await _adminService.CreateUserAsync(epost, passord, tilgangsnivaa, organisasjon, navn);
            if (success)
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("AdminView");
        }
    }
}

