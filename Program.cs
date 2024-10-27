using HUBT_Social_API.Core.Configurations;
using HUBT_Social_API.Features.Chat.ChatHubs;
using HUBT_Social_API.src.Core.Configurations;

namespace HUBT_Social_API;

public class Program
{
    public static void Main(string[] args)
    {
        // Cấu hình Swagger
        builder.Services.AddAuthorization();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // Cấu hình localization, JWT, Identity, MongoDB, SignalR, Cloudinary và SMTP
        builder.Services.ConfigureLocalization();
        builder.Services.ConfigureJwt(builder.Configuration);
        builder.Services.ConfigureIdentity(builder.Configuration);
        builder.Services.ConfigureSignalR();
        builder.Services.ConfigureCloudinary(builder.Configuration);
        builder.Services.ConfigureSMPT(builder.Configuration);
        builder.Services.FirebaseService(builder.Configuration);
        builder.Services.AddConfigureationService(builder.Configuration);

        builder.Services.AddChatMongoCollections(builder.Configuration);
        builder.Services.AddAuthMongoCollections(builder.Configuration);
    }

    private static void Services(WebApplicationBuilder builder)
    {
        // Đăng ký các dịch vụ của hệ thống
        builder.Services.RegisterApplicationServices();

        // Thêm hỗ trợ cho các controller
        builder.Services.AddControllers();
    }

    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        // Gọi hàm cấu hình và đăng ký dịch vụ
        Configures(builder);
        Services(builder);

        var app = builder.Build();

        // Cấu hình HTTP request pipeline
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseLocalization();
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();

        // Định tuyến các controller và SignalR Hub
        app.MapControllers();
        app.MapHub<ChatHub>("/chathub");

        app.Run();
    }
}