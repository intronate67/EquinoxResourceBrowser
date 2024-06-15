using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(PlanetId))]
public class Planet : BaseModel
{
    [Required]
    public int PlanetId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int SolarSystemId { get; set; }
    [Required]
    public int TypeId { get; set; }
    public int? Power { get; set; }
    public int? Workforce { get; set; }
    public int? SuperionicRate { get; set; }
    public int? MagmaticRate {  get; set; }
}