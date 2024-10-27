using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Models;

[CollectionName("user")]
public class AUser : MongoIdentityUser<Guid>
{
    public string StudentImageURL { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Language { get; set; } = "vn";
}