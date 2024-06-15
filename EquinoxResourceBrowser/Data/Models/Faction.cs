using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(FactionId))]
public class Faction : BaseModel
{
    public int? CorporationId { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;
    [Required]
    public int FactionId { get; set; }
    [Required]
    public bool IsUnique { get; set; }
    public int? MilitiaCorporationId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public float SizeFactor { get; set; }
    public int? SolarSystemId { get; set; }
    [Required]
    public int StationCount { get; set; }
    [Required]
    public int StationSystemCount { get; set; }
}
