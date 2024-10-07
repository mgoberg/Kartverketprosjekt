using kartverketprosjekt.Models;
using Microsoft.AspNetCore.Mvc;
using kartverketprosjekt.Models;

namespace YourNamespace.Controllers
{
    public class ContactController : Controller
    {
        [HttpGet]
        public IActionResult ContactPage()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Submit(ContactModel model)
        {
            if (ModelState.IsValid)
            {
                // TODO: Process the form submission (e.g., send email, save to database)
                // For now, we'll just redirect to a thank you page
                return RedirectToAction("ThankYou");
            }

            // If the model is not valid, return to the form with validation errors
            return View("ContactPage", model);
        }

        public IActionResult ThankYou()
        {
            return View();
        }
    }
}