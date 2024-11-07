using kartverketprosjekt.Data;
using kartverketprosjekt.Models;
using kartverketprosjekt.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace kartverketprosjekt.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly KartverketDbContext _context;

        public HomeController(ILogger<HomeController> logger, KartverketDbContext context)
        {
            _logger = logger;
           _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        public IActionResult ChangePassword()
        {
            return View();
        }


        // New Contact-related actions
        [HttpGet]
        public IActionResult Contact()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Contact(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: H�ndter innsendt kontaktskjema
                return RedirectToAction("ContactConfirmed");
            }

            // If the model is not valid, return to the form with validation errors
            return View(model);
        }

        public IActionResult ContactConfirmed()
        {
            return View();
        }

        
    }
}

