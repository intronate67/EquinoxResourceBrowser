namespace EquinoxResourceBrowser.Dtos.Resources
{
    public class UpgradeResource
    {
        public int TypeId { get; set; }
        public string UpgradeName { get; set; } = string.Empty;
        public int Power { get; set; }
        public int Workforce { get; set; }
        public int SuperionicRate { get; set; }
        public int MagmaticRate { get; set; }
    }
}
