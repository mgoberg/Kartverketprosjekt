using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{
    public class BrukerModel
    {
        [Key]
        public int BrukerId { get; set; } // Primary key
        [Required]
        public string Brukernavn { get; set; }
        [Required]
        public string Passord { get; set; }
        [Required]
        [EmailAddress]
        public string Epost { get; set; }
    }
}
