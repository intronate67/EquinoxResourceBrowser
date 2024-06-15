using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class Type
    {
        [JsonPropertyName("type_id")]
        public int Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;

        [JsonPropertyName("description")]
        public string Description { get; set; } = string.Empty;
    }
}