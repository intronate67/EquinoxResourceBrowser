using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class Star
    {
        [JsonPropertyName("age")]
        public long Age { get; set; }
        [JsonPropertyName("luminosity")]
        public float Luminosity { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("radius")]
        public long Radius { get; set; }
        [JsonPropertyName("solar_system_id")]
        public int SolarSystemId { get; set; }
        [JsonPropertyName("spectral_class")]
        public string SpectralClass { get; set; } = string.Empty;
        [JsonPropertyName("temperature")]
        public int Temperature { get; set; }
        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }
    }
}
