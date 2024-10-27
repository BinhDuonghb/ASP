using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using HUBT_Social_API.Features.Chat.Services.IChatServices;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.ChildChatServices;

public class FileService : IFileService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;
    private readonly Cloudinary _cloudinary;

    public FileService(IMongoCollection<ChatRoomModel> chatRooms, Cloudinary cloudinary)
    {
        _chatRooms = chatRooms;
        _cloudinary = cloudinary;
    }

    public async Task<bool> UploadFileAsync(string chatRoomId, byte[] fileData, string fileName)
    {
        var fileUrl = await UploadToStorageAsync(fileData, fileName);

        var update = Builders<ChatRoomModel>.Update.Push(cr => cr.Messages, new MessageModel
        {
            Id = Guid.NewGuid().ToString(),
            Content = $"{fileName}: {fileUrl}",
            Timestamp = DateTime.UtcNow
        });

        var result = await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRoomId, update);
        return result.ModifiedCount > 0;
    }

    private async Task<string> UploadToStorageAsync(byte[] fileBytes, string fileName)
    {
        using var stream = new MemoryStream(fileBytes);
        var uploadParams = new RawUploadParams
        {
            File = new FileDescription(fileName, stream)
        };
        var uploadResult = await _cloudinary.UploadAsync(uploadParams);
        return uploadResult.Url.ToString();
    }

    // Các phương thức khác nếu cần: xử lý video, audio, etc.
}