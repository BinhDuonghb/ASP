using AspNetCore.Identity.MongoDbCore.Models;
using MongoDbGenericRepository.Attributes;

namespace HUBT_Social_API.Features.Auth.Models;

[CollectionName("role")]
public class ARole : MongoIdentityRole<Guid>
{
}