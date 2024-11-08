namespace kartverketprosjekt.Models
{
    public class ErrorViewModel
    {

        // Modell for feilvisning
        public string? RequestId { get; set; }

        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
    }
}
