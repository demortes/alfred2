using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace AlfredBackend.Models
{
    public class AuditLogDocument
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        
        [BsonElement("userId")]
        [BsonRepresentation(BsonType.String)]
        public string UserId { get; set; } = string.Empty;
        
        [BsonElement("timestamp")]
        [BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        
        [BsonElement("eventId")]
        public string EventId { get; set; } = Guid.NewGuid().ToString();
        
        [BsonElement("action")]
        public string Action { get; set; } = string.Empty;
        
        [BsonElement("entityType")]
        public string EntityType { get; set; } = string.Empty;
        
        [BsonElement("entityName")]
        public string EntityName { get; set; } = string.Empty;
        
        [BsonElement("oldValue")]
        public string? OldValue { get; set; }
        
        [BsonElement("newValue")]
        public string? NewValue { get; set; }
        
        [BsonElement("metadata")]
        public Dictionary<string, string> Metadata { get; set; } = new();
    }
}
