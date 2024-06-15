namespace EquinoxResourceBrowser.Dtos
{
    public class UpgradeDto
    {
        public int TypeId { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Power { get; set; }
        public int Workforce { get; set; }
        public int SuperionicRate { get; set; }
        public int MagmaticRate { get; set; }
    }
}
