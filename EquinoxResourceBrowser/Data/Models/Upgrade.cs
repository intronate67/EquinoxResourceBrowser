using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(TypeId))]
public class Upgrade : BaseModel
{
    [Required]
    public int TypeId { get; set; }
    [Required]
    public int Power { get; set; }
    [Required]
    public int Workforce { get; set; }
    [Required]
    public int SuperionicRate  { get; set; }
    [Required]
    public int MagmaticRate  { get; set; }
}
