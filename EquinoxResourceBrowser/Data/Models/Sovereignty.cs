using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Data.Models;

[PrimaryKey(nameof(Id), nameof(SolarSystemId))]
public class Sovereignty : BaseModel
{
    public int? AllianceId { get; set; }
    public int? CorporationId { get; set; }
    public int? FactionId { get; set; }
    public int SolarSystemId { get; set; }
}
