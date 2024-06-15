using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EquinoxResourceBrowser.Data.Models;

public abstract class BaseModel
{
    [Required]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    [Required]
    public DateTime CreateTime { get; set; }
    public DateTime? ModifiedTime { get; set; }
}
