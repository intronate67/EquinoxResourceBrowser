namespace EquinoxResourceBrowser.Dtos
{
    public class PlanetDto
    {
        public int PlanetId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int SolarSystemId { get; set; }
        public int TypeId { get; set; }
        public int? Power { get; set; }
        public int? Workforce { get; set; }
        public int? SuperionicRate { get; set; }
        public int? MagmaticRate { get; set; }
    }
}
