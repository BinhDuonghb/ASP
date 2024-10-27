using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.IChatServices;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.ChildChatServices;

public class ImageService : IImageService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly Cloudinary _cloudinary;

    public ImageService(IMongoCollection<ChatRoomModel> chatRooms, Cloudinary cloudinary)
    {
        _chatRooms = chatRooms;
        _cloudinary = cloudinary;
    }

    public async Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData)
    {
        var imageUrl = await UploadToStorageAsync(imageData);

        var filter = Builders<ChatRoomModel>.Filter.Eq(cr => cr.Id, chatRoomId);
        var update = Builders<ChatRoomModel>.Update.Push(cr => cr.Messages, new MessageModel
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Content = imageUrl,
            Timestamp = DateTime.UtcNow
        });

        var result = await _chatRooms.UpdateOneAsync(filter, update);
        return result.ModifiedCount > 0;
    }

    public async Task<string> UploadToStorageAsync(byte[] imageBytes)
    {
        using var stream = new MemoryStream(imageBytes);
        var uploadParams = new ImageUploadParams
        {
            File = new FileDescription("image", stream),
            Transformation = new Transformation().Width(500).Height(500).Crop("fill")
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.Url.ToString();
    }

    public async Task<byte[]> ResizeImageAsync(byte[] imageData, int width, int height)
    {
        // Placeholder: Xử lý thay đổi kích thước ảnh (dùng thư viện ImageSharp, etc.)
        return await Task.FromResult(imageData); // Return resized image data
    }
}