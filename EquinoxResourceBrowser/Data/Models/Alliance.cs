using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(AllianceId))]
public class Alliance : BaseModel
{
    [Required]
    public int AllianceId { get; set; }
    [Required]
    public int CreatorCorporationId { get; set; }
    [Required]
    public int CreatorCharacterId { get; set; }
    public int? ExecutorCorporationId { get; set; }
    public int? FactionId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    [MaxLength(10)]
    public string Ticker { get; set; } = string.Empty;
    [Required]
    public DateTime DateFounded { get; set; }
}
