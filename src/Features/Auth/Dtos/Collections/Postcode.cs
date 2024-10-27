using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.src.Features.Auth.Dtos.Collections
{
    [CollectionName("Postcode")]
    public class Postcode
    {
        [BsonId]
        [BsonElement("StudentCode")]
        public string StudentCode { get; set; } = string.Empty;

        [BsonElement("Code")]
        public string Code { get; set; } = string.Empty;
        [BsonElement("ExpireTime"),BsonDateTimeOptions()]
        public DateTime ExpireTime { get; set; }
    }
}
