using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class SystemSovereignty
    {
        [JsonPropertyName("alliance_id")]
        public int? AllianceId { get; set; }
        [JsonPropertyName("corporation_id")]
        public int? CorporationId { get; set; }
        [JsonPropertyName("faction_id")]
        public int? FactionId { get; set; }
        [JsonPropertyName("system_id")]
        public int SolarSystemId { get; set; }
    }
}
