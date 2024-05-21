using Microsoft.Extensions.Logging;
using UmfaApp.Data;
using UmfaApp.Data.Tables;

namespace UmfaApp.Services
{
    public interface IPartnerService
    {
        public Task<List<Partner>> GetLocalPartners(int userId);
        public Task AddNewPartners(int userId, List<Models.Partner> partners);
    }

    public class PartnerService : IPartnerService
    {
        private readonly DbAccessor _dbAccessor;
        private readonly ILogger<PartnerService> _logger;

        public PartnerService(ILogger<PartnerService> logger, DbAccessor dbAccessor)
        {
            _logger = logger;
            _dbAccessor = dbAccessor;
        }

        public async Task<List<Partner>> GetLocalPartners(int userId)
        {
            try
            {
                return await _dbAccessor.GetPartnersByUserIdAsync(userId);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }

        public async Task AddNewPartners(int userId, List<Models.Partner> partners)
        {
            try
            {
                 await _dbAccessor.AddNewPartnersAsync(userId, partners);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
            }
        }
    }
}
