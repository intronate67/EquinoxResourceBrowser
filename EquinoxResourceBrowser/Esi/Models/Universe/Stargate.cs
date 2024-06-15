using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class Stargate
    {
        [JsonPropertyName("destination")]
        public StargateDestination Destination { get; set; } = new();
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("position")]
        public Position Position { get; set; } = new();
        [JsonPropertyName("stargate_id")]
        public int StargateId { get; set; }
        [JsonPropertyName("system_id")]
        public int SolarSystemId { get; set; }
        [JsonPropertyName("type_id")]
        public int TypeId { get; set; }
    }

    public class StargateDestination
    {
        [JsonPropertyName("stargate_id")]
        public int StargateId { get; set; }
        [JsonPropertyName("system_id")]
        public int SolarSystemId { get; set; }
    }
}
