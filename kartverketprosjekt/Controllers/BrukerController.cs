using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using kartverketprosjekt.Models; // Ensure this points to the BrukerModel class
using kartverketprosjekt.Data; // Ensure this points to your DbContext
using System.Linq; // Needed for LINQ queries to select model errors

namespace kartverketprosjekt.Controllers
{
    public class BrukerController : Controller
    {
        private readonly KartverketDbContext _context; // Specify your DbContext class here

        public BrukerController(KartverketDbContext context) // Constructor injection of DbContext
        {
            _context = context;
        }

        // GET login
        public IActionResult Login()
        {
            return View(); // Returns the login view
        }

        // POST login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginModel model)
        {
            if (ModelState.IsValid)
            {
                // Logic for validating login
                // TODO: Implement login validation logic here
            }
            return View(model); // Return the model back to the view if invalid
        }


        // POST register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrerModel model)
        {
            if (ModelState.IsValid)
            {
                var bruker = new BrukerModel
                {
                    epost = model.epost,
                    passord = model.passord,
                    tilgangsnivaa_id = 1
                };

                await _context.AddAsync(bruker);
                await _context.SaveChangesAsync();

                // Set a success message in TempData
                TempData["Message"] = "Registrering vellykket";

                // Redirect to the CorrectMap view
                return RedirectToAction("CorrectMap", "Home");
            }

            // Log the validation errors for debugging
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            return BadRequest(new { success = false, errors });
        }
    }
}
