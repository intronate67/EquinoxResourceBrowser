namespace EquinoxResourceBrowser.Dtos
{
    public class SystemDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = "N/A";
        public int TotalPower { get; set; }
        public int TotalWorkforce { get; set; }
        public int SuperionicRate { get; set; }
        public int MagmaticRate { get; set; }
        public int? SovereignAllianceId { get; set; }
        public string? SovereignAllianceName { get; set; }
        public int? SovereignCorporationId { get; set; }
        public string? SovereignCorporationName { get; set; }
        public int? SovereignFactionId { get; set; }
        public string? SovereignFacionName { get; set; }
    }
}
