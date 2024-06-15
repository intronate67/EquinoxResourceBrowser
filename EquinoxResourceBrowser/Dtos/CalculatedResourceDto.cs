namespace EquinoxResourceBrowser.Dtos
{
    public class CalculatedResourceDto
    {
        public int SolarSystemId { get; set; }
        public int? ConstellationId { get; set; }
        public int? RegionId { get; set; }
        public int TotalPower { get; set; }
        public int TotalWorkforce { get; set; }
        public int? AllianceId { get; set; }
        public int? CorporationId { get; set; }
    }
}
