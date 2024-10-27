using HUBT_Social_API.Features.Chat.Services.IChatServices;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services;

public class ChatRoomService : IChatRoomService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;

    public ChatRoomService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }

    // Thêm nhóm chat mới
    public async Task<bool> CreateGroupAsync(string nameGroup)
    {
        var newChatRoom = new ChatRoomModel
        {
            Id = Guid.NewGuid().ToString(),
            Name = nameGroup,
            CreatedAt = DateTime.Now
        };

        await _chatRooms.InsertOneAsync(newChatRoom); // Thêm nhóm vào MongoDB
        return true;
    }

    // Cập nhật tên nhóm
    public async Task<bool> UpdateGroupNameAsync(string id, string newName)
    {
        var update = Builders<ChatRoomModel>.Update.Set(c => c.Name, newName); // Cập nhật tên nhóm
        var result = await _chatRooms.UpdateOneAsync(c => c.Id == id, update); // Thực hiện cập nhật

        return result.ModifiedCount > 0; // Kiểm tra xem có thay đổi nào không
    }

    // Xóa nhóm chat
    public async Task<bool> DeleteGroupAsync(string id)
    {
        var result = await _chatRooms.DeleteOneAsync(c => c.Id == id); // Xóa nhóm theo ID
        return result.DeletedCount > 0; // Kiểm tra xem có nhóm nào bị xóa không
    }

    // Lấy thông tin nhóm theo ID
    public async Task<ChatRoomModel> GetGroupByIdAsync(string id)
    {
        return await _chatRooms.Find(c => c.Id == id).FirstOrDefaultAsync(); // Tìm nhóm theo ID
    }

    // Lấy tất cả nhóm chat
    public async Task<List<ChatRoomModel>> GetAllGroupsAsync()
    {
        return await _chatRooms.Find(_ => true).ToListAsync(); // Lấy tất cả nhóm từ MongoDB
    }
}