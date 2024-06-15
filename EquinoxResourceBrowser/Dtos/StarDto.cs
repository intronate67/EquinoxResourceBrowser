namespace EquinoxResourceBrowser.Dtos
{
    public class StarDto
    {
        public int StarId { get; set; }
        public long Age { get; set; }
        public float Luminosity { get; set; }
        public string Name { get; set; } = string.Empty;
        public long Radius { get; set; }
        public int SolarSystemId { get; set; }
        public string SpectralClass { get; set; } = string.Empty;
        public int Temperature { get; set; }
        public int TypeId { get; set; }
        public int? Power { get; set; }
    }
}
