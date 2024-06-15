namespace EquinoxResourceBrowser.Dtos;

public class RegionDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "N/A";
    public int TotalPower { get; set; }
    public int TotalWorkforce { get; set; }
    public int SuperionicRate { get; set; }
    public int MagmaticRate { get; set; }
}
