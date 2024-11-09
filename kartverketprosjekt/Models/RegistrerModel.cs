using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{
    public class RegistrerModel
    {
        // Modell for registrering
        [Required(ErrorMessage = "Passord er påkrevd.")]
        public string? passord { get; set; }

        [Required(ErrorMessage = "Epost er påkrevd.")]
        [EmailAddress(ErrorMessage = "Ugyldig epostadresse.")]
        public string epost { get; set; }

        public string? navn { get; set; }
    }
}