using HUBT_Social_API.Features.Auth.Dtos.Collections;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.src.Core.Configurations;

public static class MongoDbConfiguration
{
    public static IServiceCollection AddAuthMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        var client = new MongoClient(configuration.GetConnectionString("AuthService"));
        var database = client.GetDatabase("HUBT_Management");

        services.AddScoped<IMongoCollection<RefreshToken>>(s => database.GetCollection<RefreshToken>("RefreshTokens"));
        services.AddScoped<IMongoCollection<Postcode>>(s => database.GetCollection<Postcode>("Postcode"));


        return services;
    }

    public static IServiceCollection AddChatMongoCollections(this IServiceCollection services,
        IConfiguration configuration)
    {
        var chatClient = new MongoClient(configuration.GetConnectionString("ChatService"));
        var chatDatabase = chatClient.GetDatabase("HUBT_Management");

        services.AddScoped<IMongoCollection<ChatRoomModel>>(s => chatDatabase.GetCollection<ChatRoomModel>("ChatRoom"));

        return services;
    }
}