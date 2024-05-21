using Microsoft.Extensions.Logging;
using UmfaApp.Data;
using UmfaApp.Data.Tables;
using Action = UmfaApp.Enums.Action;

namespace UmfaApp.Services
{
    public interface IActionLogService 
    {
        public Task AddLog(Action action, int userId, string data, Guid? readingListId, string error = null);
        public Task<List<ActionLog>> GetLogs();

        public Task<List<ActionLog>> GetLogsByReadingListId(Guid id);
    }

    public class ActionLogService : IActionLogService
    {
        private readonly DbAccessor _dbAccessor;

        public ActionLogService(ILogger<ActionLogService> logger, DbAccessor dbAccessor)
        {
            _dbAccessor = dbAccessor;
        }

        public async Task AddLog(Action action, int userId, string data, Guid? readingListId, string error = null)
        {
            await _dbAccessor.AddLog(new ActionLog(action, userId, data, readingListId, error));
        }

        public async Task<List<ActionLog>> GetLogs()
        {
            return await _dbAccessor.GetLogs();
        }

        public async Task<List<ActionLog>> GetLogsByReadingListId(Guid id)
        {
            return await _dbAccessor.GetLogsByReadingListId(id);
        }
    }
}
