using EquinoxResourceBrowser.Dtos;

namespace EquinoxResourceBrowser.Interfaces
{
    public interface IUpgradeService
    {
        Task<List<UpgradeDto>> GetUpgrades();
    }
}
