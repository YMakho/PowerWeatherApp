using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace PowerWeatherApp.Infrastructure.Persistence.MongoEntities
{
    public sealed class CachedForecast
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string Id { get; set; }
        public string Coordinates { get; set; }
        public string Location { get; set; }
        public string RawJson { get; set; } 
        public DateTime CachedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
