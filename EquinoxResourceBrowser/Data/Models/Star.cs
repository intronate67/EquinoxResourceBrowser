using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(StarId))]
public class Star : BaseModel
{
    [Required]
    public int StarId { get; set; }
    [Required]
    public long Age { get; set; }
    [Required]
    public float Luminosity { get; set; }
    public string Name { get; set; } = string.Empty;
    [Required]
    public long Radius { get; set; }
    [Required]
    public int SolarSystemId { get; set; }
    [Required]
    public string SpectralClass { get; set; } = string.Empty;
    [Required]
    public int Temperature { get; set; }
    [Required]
    public int TypeId { get; set; }
    public int? Power { get; set; }
}
