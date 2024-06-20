using EquinoxResourceBrowser.Data;
using EquinoxResourceBrowser.Dtos;
using EquinoxResourceBrowser.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace EquinoxResourceBrowser.Services
{
    public class UpgradeService : IUpgradeService
    {
        private readonly ILogger<UpgradeService> _logger;
        private readonly ResourceContext _ctx;

        public UpgradeService(ILogger<UpgradeService> logger, ResourceContext ctx)
        {
            _logger = logger;
            _ctx = ctx;
        }

        public async Task<List<UpgradeDto>> GetUpgrades()
        {
            try
            {
                return await (from u in _ctx.Upgrades
                       join t in _ctx.Types on u.TypeId equals t.TypeId
                       select new UpgradeDto
                       {
                           TypeId = u.TypeId,
                           Name = t.Name,
                           Power = u.Power,
                           Workforce = u.Workforce,
                           SuperionicRate = u.SuperionicRate,
                           MagmaticRate = u.MagmaticRate
                       }).ToListAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred ({ErrorMessage}) while getting upgrades", ex.Message);
            }

            return [];
        }
    }
}
