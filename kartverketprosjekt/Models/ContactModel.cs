using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{

    //TODO: FJERN?
    public class ContactModel
    {
        [Required(ErrorMessage = "Navn er påkrevd")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email er påkrevd")]
        [EmailAddress(ErrorMessage = "Ugyldig epost")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Emne er påkrevd")]
        public string Subject { get; set; }

        [Required(ErrorMessage = "Beskrivelse er påkrevd")]
        public string Message { get; set; }
    }
}