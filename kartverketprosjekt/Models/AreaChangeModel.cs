namespace kartverketprosjekt.Models
{
    public class AreaChangeModel
    {
        public required string ID { get; set; }
        public required string GeoJSON { get; set; }
        public string ? Description { get; set; } // ? betyr at feltet er nullable, kan vurderes om det skal være required
        public required string LayerUrl { get; set; }
    }
}
