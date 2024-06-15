namespace EquinoxResourceBrowser.Dtos.Resources
{
    public class StarResource
    {
        public int StarId { get; set; }
        public string RegionName { get; set; } = string.Empty;
        public string SolarSystemName { get; set; } = string.Empty;
        public string StarName { get; set; } = string.Empty;
        public int Power { get; set; }
    }
}