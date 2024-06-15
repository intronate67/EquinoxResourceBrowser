using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace EquinoxResourceBrowser.Data.Models
{
    [PrimaryKey(nameof(Id), nameof(StargateId))]
    public class Stargate : BaseModel
    {
        [Required]
        public int StargateId { get; set; }
        [Required]
        public int DestinationStargateId { get; set; }
        [Required]
        public int DestinationSystemId { get; set; }
        [Required]
        public string Name { get; set; } = string.Empty;
        [Required]
        public double X { get; set; }
        [Required]
        public double Y { get; set; }
        [Required]
        public double Z { get; set; }
        [Required]
        public int SolarSystemId { get; set; }
        [Required]
        public int TypeId { get; set; }
    }
}
