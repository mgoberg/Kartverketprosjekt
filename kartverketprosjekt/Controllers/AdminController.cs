using kartverketprosjekt.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Text;
using kartverketprosjekt.Models;
using kartverketprosjekt.Services.Admin;

// ****************************************************************************************************************************
// ******AdminController er en controller som håndterer alle funksjoner som kun skal være tilgjengelig for Administrator.******
// ****************************************************************************************************************************

namespace kartverketprosjekt.Controllers

{


    // Gir tilgang til AdminController kun for brukere med rolle 4 (admin brukere)
    [Authorize(Roles = "4")]
    public class AdminController : Controller
    {
        private readonly IAdminService _adminService;

        
        public AdminController(IAdminService adminService)
        {
           
            _adminService = adminService;
        }

        // Metode for å vise admin viewet.
        public IActionResult AdminView()
        {
            var stats = _adminService.GetAdminViewStats();
            ViewData["CaseCount"] = stats.CaseCount;
            ViewData["UserCount"] = stats.UserCount;
            ViewData["OpenCasesUnbehandlet"] = stats.OpenCasesUnbehandlet;
            ViewData["OpenCasesUnderBehandling"] = stats.OpenCasesUnderBehandling;
            ViewData["OpenCasesAvvist"] = stats.OpenCasesAvvist;
            ViewData["OpenCasesArkivert"] = stats.OpenCasesArkivert;
            ViewData["ClosedCases"] = stats.ClosedCases;

            return View(stats.Users);
        }

        //Metode for å endre tilgangsnivået til en bruker fra admin bruker.
        [HttpPost]
        public IActionResult UpdateAccess(string userId, int newAccessLevel)
        {
            if (_adminService.UpdateUserAccess(userId, newAccessLevel, out var message))
            {
                TempData["SuccessMessage"] = message;
            }
            else
            {
                TempData["ErrorMessage"] = message;
            }

            return RedirectToAction("AdminView");
        }

        // Metode for å slette en bruker fra admin brukeren.
        [HttpPost]
        public IActionResult SlettBruker(string epost)
        {
            var loggedInUserEmail = User.Identity.Name;
            if (_adminService.DeleteUser(epost, loggedInUserEmail, out var errorMessage))
            {
                TempData["SuccessMessage"] = errorMessage;
                return RedirectToAction("AdminView");
            }

            ViewBag.ErrorMessage = errorMessage;
            var users = _adminService.GetAdminViewStats().Users; // Reuse the users list
            return View("AdminView", users);
        }


        // Metode for å opprette en ny bruker fra admin brukeren.
        [HttpPost]
        public IActionResult OpprettBruker(string epost, string passord, int tilgangsnivaa, string organisasjon, string? navn)
        {
            if (_adminService.CreateUser(epost, passord, tilgangsnivaa, organisasjon, navn, out var errorMessage))
            {
                TempData["SuccessMessage"] = errorMessage;
            }
            else
            {
                TempData["ErrorMessage"] = errorMessage;
            }

            return RedirectToAction("AdminView");
        }

    }


}
