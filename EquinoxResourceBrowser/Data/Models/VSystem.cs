namespace EquinoxResourceBrowser.Data.Models
{
    public class VSystem
    {
        public int SolarSystemId { get; set; }
        public string Name { get; set; } = "N/A";
        public int ConstellationId { get; set; }
        public int RegionId { get; set; }
        public int? TotalPower { get; set; }
        public int? TotalWorkforce { get; set; }
        public int? TotalSuperionicIce { get; set; }
        public int? TotalMagmaticGas { get; set; }
        public int? SovereignAllianceId { get; set; }
        public string? SovereignAllianceName { get; set; }
        public int? SovereignCorporationId { get; set; }
        public string? SovereignCorporationName { get; set; }
        public int? SovereignFactionId { get; set; }
        public string? SovereignFactionName { get; set; }
    }
}
