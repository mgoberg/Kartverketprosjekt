using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{
    public class RegistrerModel
    {
        [Required(ErrorMessage = "Brukernavn er påkrevd.")]
        public string Brukernavn { get; set; }

        [Required(ErrorMessage = "Passord er påkrevd.")]
        public string Passord { get; set; }

        [Required(ErrorMessage = "Epost er påkrevd.")]
        [EmailAddress(ErrorMessage = "Ugyldig epostadresse.")]
        public string Epost { get; set; }
    }
}