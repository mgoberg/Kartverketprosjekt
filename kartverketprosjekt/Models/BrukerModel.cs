using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{
    public class BrukerModel
    {
        [Key]
        [EmailAddress]
        public string epost { get; set; }

        public string? navn { get; set; }
        [Required]
        public string passord { get; set; }

        public int tilgangsnivaa_id { get; set; } = 1; // Standard verd i 1

        public string? organisasjon { get; set; } = "Ingen"; // Standard verdi er Kartverket

    }
}
