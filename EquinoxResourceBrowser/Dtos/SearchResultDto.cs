using EquinoxResourceBrowser.Enums;

namespace EquinoxResourceBrowser.Dtos
{
    public class SearchResultDto
    {
        public int Id { get; set; }
        public int? ConstellationId { get; set; }
        public int? RegionId { get; set; }
        public string Name { get; set; } = string.Empty;
        public SearchResultType Type { get; set; } = SearchResultType.None;
    }
}
