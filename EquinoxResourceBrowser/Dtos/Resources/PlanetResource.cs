namespace EquinoxResourceBrowser.Dtos.Resources
{
    public class PlanetResource
    {
        public int PlanetId { get; set; }
        public string RegionName  { get; set; } = string.Empty;
        public string SolarSystemName { get; set; } = string.Empty;
        public string PlanetName { get; set; } = string.Empty;
        public int Power { get; set; }
        public int Workforce { get; set; }
        public int SuperionicRate { get; set; }
        public int MagmaticRate { get; set; }
    }
}
