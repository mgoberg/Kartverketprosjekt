using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace kartverketprosjekt.Models
{
    public class SakModel
    {
        public int id { get; set; }
        public string ? epost_bruker { get; set; }
        [Required]
        public string beskrivelse { get; set; }
        public string ? vedlegg { get; set; }
        public string ? geojson_data { get; set; }
        public int ? kommune_id { get; set; }
        public string type_feil { get; set; }
        public string ? status { get; set; } = "Påbegynt";
        public DateTime opprettet_dato { get; set; } = DateTime.Now;
        public string layerurl { get; set; }

    }
}
