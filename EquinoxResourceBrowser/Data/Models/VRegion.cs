namespace EquinoxResourceBrowser.Data.Models
{
    public class VRegion
    {
        public int Id { get; set; }
        public string Name { get; set; } = "N/A";
        public int TotalPower { get; set; }
        public int TotalWorkforce { get; set; }
        public int TotalSuperionicIce { get; set; }
        public int TotalMagmaticGas { get; set; }
    }
}
