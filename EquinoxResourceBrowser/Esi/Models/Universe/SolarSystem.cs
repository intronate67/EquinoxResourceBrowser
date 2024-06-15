using System.Text.Json.Serialization;

namespace EquinoxResourceBrowser.Esi.Models.Universe
{
    public class SolarSystem
    {
        [JsonPropertyName("constellation_id")]
        public int ConstellationId { get; set; }
        [JsonPropertyName("name")]
        public string Name { get; set; } = string.Empty;
        [JsonPropertyName("planets")]
        public SystemPlanet[] Planets { get; set; } = [];
        [JsonPropertyName("security_class")]
        public string? SecurityClass { get; set; }
        [JsonPropertyName("security_status")]
        public float SecurityStatus { get; set; }
        [JsonPropertyName("star_id")]
        public int? StarId { get; set; }
        [JsonPropertyName("stargates")]
        public int[]? Stargates { get; set; }
        [JsonPropertyName("system_id")]
        public int Id { get; set; }
    }

    public class SystemPlanet
    {
        [JsonPropertyName("asteroid_belts")]
        public int[] AsteroidBelts { get; set; } = [];
        [JsonPropertyName("moons")]
        public int[] Moons { get; set; } = [];
        [JsonPropertyName("planet_id")]
        public int PlanetId { get; set; }
    }
}
