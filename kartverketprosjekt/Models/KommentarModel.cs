namespace kartverketprosjekt.Models
{
    public class KommentarModel
    {
        public int Id { get; set; }
        public int SakID { get; set; }
        public string Tekst { get; set; }
        public DateTime Dato { get; set; }
        public string? Epost { get; set; }

        // Navigasjonsfelt, hvis nødvendig
        public SakModel Sak { get; set; }
    }

}
