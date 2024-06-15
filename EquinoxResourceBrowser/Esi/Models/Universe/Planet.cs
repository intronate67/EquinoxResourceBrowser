using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class Planet
    {
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("planet_id")]
        public int Id { get; set; }
        [JsonPropertyName("system_id")]
        public int SolarSystemId { get; set; }
        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }
    }
}
