using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Localization;

namespace kartverketprosjekt.Resources.Views.Home
{
    public class IndexResource
    {
        private readonly IStringLocalizer<IndexResource> _localizer;

        public IndexResource(IStringLocalizer<IndexResource> localizer)
        {
            _localizer = localizer;
        }

        public string KartverketsNyeRapporteringstjeneste => _localizer["KartverketsNyeRapporteringstjeneste"];
        public string BidraMedForbedreKartgrunnlaget => _localizer["BidraMedForbedreKartgrunnlaget"];
        public string TilKartet => _localizer["TilKartet"];
        public string KortOmTjenesten => _localizer["KortOmTjenesten"];
        public string KartverketTilbyrEnTjeneste => _localizer["KartverketTilbyrEnTjeneste"];
        public string Oblig1 => _localizer["Oblig1"];
        public string LoggInn => _localizer["LoggInn"];
        public string Epost => _localizer["Epost"];
        public string Passord => _localizer["Passord"];
        public string Registrer => _localizer["Registrer"];
        public string FortsettUtenInnlogging => _localizer["FortsettUtenInnlogging"];
    }
}