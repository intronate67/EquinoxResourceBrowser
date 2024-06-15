using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(ConstellationId))]
public class Constellation : BaseModel
{
    [Required]
    public int ConstellationId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public int RegionId { get; set; }
}
