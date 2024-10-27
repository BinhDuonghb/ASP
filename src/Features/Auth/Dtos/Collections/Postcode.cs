using MongoDB.Bson.Serialization.Attributes;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Dtos.Collections;

[CollectionName("Postcode")]
public class Postcode
{
    [BsonId] [BsonElement("Email")] public string Email { get; set; } = string.Empty;

    [BsonElement("Code")] public string Code { get; set; } = string.Empty;

    [BsonElement("ExpireTime")]
    [BsonDateTimeOptions]
    public DateTime ExpireTime { get; set; }
}