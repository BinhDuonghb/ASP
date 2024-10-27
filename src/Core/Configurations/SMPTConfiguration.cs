using HUBT_Social_API.Core.Settings;

namespace HUBT_Social_API.Core.Configurations;

public static class SMPTConfiguration
{
    public static IServiceCollection ConfigureSMPT(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<SMPTSetting>(configuration.GetSection("SMPTSetting"));
        return services;
    }
}