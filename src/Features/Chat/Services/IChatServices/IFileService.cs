namespace HUBT_Social_API.Features.Chat.Services.IChatServices;

public interface IImageService
{
    Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData);
}