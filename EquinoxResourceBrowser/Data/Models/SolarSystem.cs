using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(SolarSystemId))]
public class SolarSystem : BaseModel
{
    [Required]
    public int SolarSystemId { get; set; }
    [Required]
    public int ConstellationId { get; set; }
    public int? StarId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public float SecurityStatus { get; set; }
    [MaxLength(2)]
    public string SecurityClass { get; set; } = string.Empty;
}
