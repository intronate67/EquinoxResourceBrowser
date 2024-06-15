using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(CorporationId))]
public class Corporation : BaseModel
{
    [Required]
    public int CorporationId { get; set; }
    public int? AllianceId { get; set; }
    public int? FactionId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    [Required]
    [MaxLength(10)]
    public string Ticker { get; set; } = string.Empty;
    public DateTime? DateFounded { get; set; }
}
