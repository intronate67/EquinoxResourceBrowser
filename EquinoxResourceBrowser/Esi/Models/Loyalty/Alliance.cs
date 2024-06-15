using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Loyalty
{
    public class Alliance
    {
        [JsonPropertyName("creator_corporation_id")]
        public int CreatorCorporationId { get; set; }
        [JsonPropertyName("creator_id")]
        public int CreatorCharacterId { get; set; }
        [JsonPropertyName("date_founded")]
        public DateTime CreationDate { get; set; }
        [JsonPropertyName("executor_corporation_id")]
        public int? ExecutorCorporationId { get; set; }
        [JsonPropertyName("faction_id")]
        public int? FactionId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("ticker")]
        public string Ticker { get; set; } = string.Empty;
    }
}
