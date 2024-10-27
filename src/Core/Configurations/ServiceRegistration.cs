using HUBT_Social_API.Features.Auth.Services;
using HUBT_Social_API.Features.Auth.Services.ChildServices;
using HUBT_Social_API.Features.Auth.Services.IAuthServices;
using HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;
using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.Services;
using HUBT_Social_API.Features.Chat.Services.ChildChatServices;
using HUBT_Social_API.Features.Chat.Services.IChatServices;

namespace HUBT_Social_API.Core.Configurations;

public static class ServiceRegistration
{
    public static IServiceCollection RegisterApplicationServices(this IServiceCollection services)
    {
        // Đăng ký các dịch vụ
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddScoped<IChatService, ChatService>();
        services.AddScoped<IMessageService, MessageService>();
        services.AddScoped<IFileService, FileService>();
        services.AddScoped<IImageService, ImageService>();
        services.AddScoped<IChatMessageHub, ChatMessageHub>();
        services.AddScoped<IChatFileHub, ChatFileHub>();
        services.AddScoped<ILoginService, LoginService>();
        services.AddScoped<IRegisterService, RegisterService>();
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<RegisterService>();
        services.AddScoped<IChatRoomService, ChatRoomService>();

/*            services.AddScoped<IUserManagerS, UserManagerS>();
*/
        services.AddScoped<IEmailService, EmailService>();


        return services;
    }
}