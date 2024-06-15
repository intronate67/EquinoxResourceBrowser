using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(TypeId))]
public class Type : BaseModel
{
    [Required]
    public int TypeId { get; set; }
    [Required]
    public string Name { get; set; } = string.Empty;
    public string? Description  { get; set; }
}
