using FireSharp.Config;
using FireSharp.Interfaces;

namespace HUBT_Social_API.Core.Configurations;

public static class FirebaseConfiguration
{
    public static IServiceCollection FirebaseService(this IServiceCollection services, IConfiguration config)
    {
        services.AddSingleton<IFirebaseConfig>(s => new FirebaseConfig
        {
            AuthSecret = config.GetValue<string>("FirebaseSetting:AuthSecret"),
            BasePath = config.GetValue<string>("FirebaseSetting:BasePath")
        });

        return services;
    }
}