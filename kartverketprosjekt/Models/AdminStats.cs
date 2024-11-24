namespace kartverketprosjekt.Models
{
    public class AdminStats
    {
        public int CaseCount { get; set; }
        public int UserCount { get; set; }
        public int OpenCasesUnbehandlet { get; set; }
        public int OpenCasesUnderBehandling { get; set; }
        public int OpenCasesAvvist { get; set; }
        public int OpenCasesArkivert { get; set; }
        public int ClosedCases { get; set; }
        public List<BrukerModel> Users { get; set; }
    }
}
