using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(RegionId))]
public class Region : BaseModel
{
    [Required]
    public int RegionId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
