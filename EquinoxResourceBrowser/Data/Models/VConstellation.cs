namespace EquinoxResourceBrowser.Data.Models
{
    public class VConstellation
    {
        public int ConstellationId { get; set; }
        public string Name { get; set; } = "N/A";
        public int RegionId { get; set; }
        public int? TotalPower { get; set; }
        public int? TotalWorkforce { get; set; }
        public int? TotalSuperionicIce { get; set; }
        public int? TotalMagmaticGas { get; set; }
    }
}
