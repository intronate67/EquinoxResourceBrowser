using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class Region
    {
        [JsonPropertyName("constellations")]
        public int[] Constellations { get; set; } = [];
        [JsonPropertyName("description")]
        public string? Description { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("region_id")]
        public int Id { get; set; }
    }
}
