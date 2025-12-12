using MongoDB.Driver;
using AlfredBackend.Models;

namespace AlfredBackend.Services
{
    public class MongoAuditLogService : IAuditLogService
    {
        private readonly IMongoCollection<AuditLogDocument> _collection;
        
        public MongoAuditLogService(IMongoClient mongoClient)
        {
            var database = mongoClient.GetDatabase("alfred_audit");
            _collection = database.GetCollection<AuditLogDocument>("audit_logs");
            
            // Create indexes for efficient querying
            CreateIndexes();
        }
        
        private void CreateIndexes()
        {
            // Compound index: userId + timestamp (descending)
            var userTimeIndex = Builders<AuditLogDocument>.IndexKeys
                .Ascending(x => x.UserId)
                .Descending(x => x.Timestamp);
            _collection.Indexes.CreateOne(new CreateIndexModel<AuditLogDocument>(userTimeIndex));
            
            // Action index
            var actionIndex = Builders<AuditLogDocument>.IndexKeys.Ascending(x => x.Action);
            _collection.Indexes.CreateOne(new CreateIndexModel<AuditLogDocument>(actionIndex));
            
            // Timestamp index for time-based queries
            var timestampIndex = Builders<AuditLogDocument>.IndexKeys.Descending(x => x.Timestamp);
            _collection.Indexes.CreateOne(new CreateIndexModel<AuditLogDocument>(timestampIndex));
        }
        
        public async Task LogAsync(AuditLogDocument log)
        {
            log.Timestamp = DateTime.UtcNow;
            log.EventId = Guid.NewGuid().ToString();
            await _collection.InsertOneAsync(log);
        }
        
        public async Task<List<AuditLogDocument>> GetUserLogsAsync(string userId, DateTime? from = null, DateTime? to = null, int limit = 1000)
        {
            var filterBuilder = Builders<AuditLogDocument>.Filter;
            var filter = filterBuilder.Eq(x => x.UserId, userId);
            
            if (from.HasValue)
                filter &= filterBuilder.Gte(x => x.Timestamp, from.Value);
            if (to.HasValue)
                filter &= filterBuilder.Lte(x => x.Timestamp, to.Value);
            
            return await _collection.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Limit(limit)
                .ToListAsync();
        }
        
        public async Task<List<AuditLogDocument>> GetLogsByActionAsync(string action, int limit = 100)
        {
            var filter = Builders<AuditLogDocument>.Filter.Eq(x => x.Action, action);
            
            return await _collection.Find(filter)
                .SortByDescending(x => x.Timestamp)
                .Limit(limit)
                .ToListAsync();
        }
    }
}
