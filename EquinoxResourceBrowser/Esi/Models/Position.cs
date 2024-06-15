using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models
{
    public class Position
    {
        [JsonPropertyName("x")]
        public double X { get; set; }
        [JsonPropertyName("y")]
        public double Y { get; set; }
        [JsonPropertyName("z")]
        public double Z { get; set; }
    }
}
