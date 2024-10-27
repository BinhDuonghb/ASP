using AspNetCore.Identity.MongoDbCore.Extensions;
using AspNetCore.Identity.MongoDbCore.Infrastructure;
using HUBT_Social_API.Features.Auth.Models;
using Microsoft.AspNetCore.Identity;

namespace HUBT_Social_API.Core.Configurations;

public static class IdentityConfiguration
{
    public static IServiceCollection ConfigureIdentity(this IServiceCollection services, IConfiguration configuration)
    {
        var mongodbConfig = new MongoDbIdentityConfiguration
        {
            MongoDbSettings = new MongoDbSettings
            {
                ConnectionString = configuration.GetSection("ConnectionStrings:AuthService").Get<string>(),
                DatabaseName = "HUBTSocialAuth"
            },
            IdentityOptionsAction = option =>
            {
                option.Password.RequireDigit = false;
                option.Password.RequiredLength = 8;
                option.Password.RequireNonAlphanumeric = true;
                option.Password.RequireLowercase = false;

                option.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                option.Lockout.MaxFailedAccessAttempts = 5;

                option.User.RequireUniqueEmail = true;
            }
        };

        services.ConfigureMongoDbIdentity<AUser, ARole, Guid>(mongodbConfig)
            .AddUserManager<UserManager<AUser>>()
            .AddSignInManager<SignInManager<AUser>>()
            .AddRoleManager<RoleManager<ARole>>()
            .AddDefaultTokenProviders();
        return services;
    }
}