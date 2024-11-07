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
        public string? Kommunenavn { get; set; }
        public string? Kommunenummer { get; set; }
        public string? Fylkesnavn { get; set; }
        public string? Fylkesnummer { get; set; }
        // Relasjon til Kommentarer
        public ICollection<KommentarModel> Kommentarer { get; set; } = new List<KommentarModel>();

        public bool status_endret { get; set; } = false;

        // Nytt felt for å indikere om saken er prioritert
        public bool IsPriority { get; set; } = false;

    }
}

