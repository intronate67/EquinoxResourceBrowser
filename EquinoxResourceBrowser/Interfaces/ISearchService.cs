using EquinoxResourceBrowser.Dtos;

namespace EquinoxResourceBrowser.Interfaces
{
    public interface ISearchService
    {
        Task<List<SearchResultDto>> LoadUniverse();
    }
}
