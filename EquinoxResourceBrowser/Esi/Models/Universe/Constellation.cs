using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class Constellation
    {
        [JsonPropertyName("constellation_id")]
        public int Id { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("region_id")]
        public int RegionId { get; set; }
        [JsonPropertyName("systems")]
        public int[] SolarSystems { get; set; } = [];
    }
}
