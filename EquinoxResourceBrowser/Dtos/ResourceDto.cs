namespace EquinoxResourceBrowser.Dtos;

public class ResourceDto
{
    public StarDto? Star { get; set; }
    public List<PlanetDto> Planets { get; set; } = [];
}
