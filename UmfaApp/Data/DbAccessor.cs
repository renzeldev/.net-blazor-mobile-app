using SQLite;
using UmfaApp.Data.Tables;
using UmfaApp.Services;
using UmfaApp.Settings;

namespace UmfaApp.Data
{
    public class DbAccessor
    {
        SQLiteAsyncConnection Database;

        public DbAccessor()
        {
        }

        async Task Init()
        {
            if (Database is not null)
                return;

            Database = new SQLiteAsyncConnection(DbSettings.DatabasePath, DbSettings.Flags);
            await Database.CreateTableAsync<ReadingList>();
            await Database.CreateTableAsync<ReadingListRequest>();
            await Database.CreateTableAsync<Reading>();
            await Database.CreateTableAsync<Partner>();
            await Database.CreateTableAsync<ActionLog>();
        }

        public async Task<int> AddLog(ActionLog log)
        {
            await Init();

            return await Database.InsertAsync(log);
        }

        public async Task<List<ActionLog>> GetLogs()
        {
            await Init();

            return await Database.Table<ActionLog>().OrderByDescending(l => l.ActionDate).ToListAsync();
        }

        public async Task<List<ActionLog>> GetLogsByReadingListId(Guid id)
        {
            await Init();

            return await Database.Table<ActionLog>().Where(l => l.ReadingListId.Equals(id)).OrderByDescending(l => l.ActionDate).ToListAsync();
        }


        public async Task AddNewPartnersAsync(int userId, List<Models.Partner> partners)
        {
            await Init();
            var existingPartners = await GetPartnersByUserIdAsync(userId);

            await Database.InsertAllAsync(partners.Where(p => !existingPartners.Select(ep => ep.PartnerId).Contains(p.PartnerId)).Select(p => new Partner { UserId = userId, Name = p.Name, PartnerId = p.PartnerId }));
        }

        public async Task<List<Partner>> GetPartnersByUserIdAsync(int userId)
        {
            await Init();
            return await Database.Table<Partner>().Where(p => p.UserId == userId).ToListAsync();
        }

        public async Task<List<Reading>> GetReadingsByReadingListRequestIdAsync(Guid readingListRequestId)
        {
            await Init();
            return await Database.Table<Reading>().Where(r => r.ReadingListRequestId == readingListRequestId).ToListAsync();
        }

        public async Task<List<Reading>> GetReadingsByReadingListIdAsync(Guid readingListId)
        {
            await Init();
            var requestsIds = (await GetReadingListRequestsAsync(readingListId)).Select(r => r.Id);
            return await Database.Table<Reading>().Where(r => requestsIds.Contains(r.ReadingListRequestId)).ToListAsync();
        }

        public async Task<List<ReadingList>> GetReadingListsAsync(int user, int partnerId)
        {
            await Init();
            return await Database.Table<ReadingList>().Where(rl => rl.UserId == user && rl.PartnerId == partnerId).ToListAsync();
        }

        public async Task<List<ReadingListRequest>> GetReadingListRequestsAsync(Guid readingListId)
        {
            await Init();
            return await Database.Table<ReadingListRequest>().Where(rlr => rlr.ReadingListId == readingListId).ToListAsync();
        }

        public async Task<List<ReadingListRequest>> GetReadingListRequestsAsync(List<Guid> readingListIds)
        {
            await Init();
            return await Database.Table<ReadingListRequest>().Where(rlr => readingListIds.Contains(rlr.ReadingListId)).ToListAsync();
        }

        public async Task<ReadingListRequest> GetReadingListRequestAsync(Guid Id)
        {
            await Init();
            return await Database.Table<ReadingListRequest>().FirstOrDefaultAsync(rlr => rlr.Id == Id);
        }

        // readings maintenance

        public async Task<int> InsertReadingsAsync(List<Reading> readings)
        {
            await Init();
            return await Database.InsertAllAsync(readings);
        }

        public async Task<int> UpdateReadingAsync(Reading reading)
        {
            await Init();
            return await Database.UpdateAsync(reading);
        }

        public async Task<int> DeleteReadingAsync(Reading item)
        {
            await Init();
            return await Database.DeleteAsync(item);
        }

        // reading list requests maintenence
        public async Task<int> UpdateReadingListRequestAsync(ReadingListRequest readingListRequest)
        {
            await Init();

            return await Database.UpdateAsync(readingListRequest);
        }
        public async Task InsertReadingListRequestAsync(ReadingListRequest readingListRequest, List<Reading> readings)
        {
            await Init();

            await Database.InsertAsync(readingListRequest);
            await InsertReadingsAsync(readings);
        }

        public async Task<int> InsertReadingListRequestAsync(ReadingListRequest readingListRequest)
        {
            await Init();

            return await Database.InsertAsync(readingListRequest);
        }

        public async Task DeleteReadingListRequestCascadingAsync(ReadingListRequest readingListRequest)
        {
            await Init();
            await Database.DeleteAsync(readingListRequest);
            var readings = await GetReadingsByReadingListRequestIdAsync(readingListRequest.Id);

            foreach (var reading in readings)
            {
                await DeleteReadingAsync(reading);
            }
        }

        public async Task DeleteReadingListRequestAsync(ReadingListRequest readingListRequest)
        {
            await Init();
            await Database.DeleteAsync(readingListRequest);
        }


        // reading lists maintenance
        public async Task<int> InsertReadingListAsync(ReadingList readingList)
        {
            await Init();

            return await Database.InsertAsync(readingList);
        }

        public async Task DeleteReadingListAsync(ReadingList readingList)
        {
            await Init();
            await Database.DeleteAsync(readingList);
            var requests = await GetReadingListRequestsAsync(readingList.Id);
            var readings = await GetReadingsByReadingListIdAsync(readingList.Id);
            foreach (var request in requests)
            {
                await DeleteReadingListRequestAsync(request);
            }

            foreach (var reading in readings)
            {
                await DeleteReadingAsync(reading);
            }
        }
    }
}
