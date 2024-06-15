using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Loyalty
{
    public class Faction
    {
        [JsonPropertyName("corporation_id")]
        public int CorporationId { get; set; }
        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
        [JsonPropertyName("faction_id")]
        public int FactionId { get; set; }
        [JsonPropertyName("is_unique")]
        public bool IsUnique { get; set; }
        [JsonPropertyName("militia_corporation_id")]
        public int? MilitiaCorporationId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("size_factor")]
        public float SizeFactor { get; set; }
        [JsonPropertyName("solar_system_id")]
        public int? SolarSystemId { get; set; }
        [JsonPropertyName("station_count")]
        public int StationCount { get; set; }
        [JsonPropertyName("station_system_count")]
        public int StationSystemCount { get; set; }
    }
}