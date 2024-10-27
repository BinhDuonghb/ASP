using HUBT_Social_API.Features.Chat.Services.IChatServices;
using HUBTSOCIAL.Src.Features.Chat.Models;
using MongoDB.Driver;

namespace HUBT_Social_API.Features.Chat.Services.ChildChatServices;

public class MessageService : IMessageService
{
    private readonly IMongoCollection<ChatRoomModel> _chatRooms;

    public MessageService(IMongoCollection<ChatRoomModel> chatRooms)
    {
        _chatRooms = chatRooms;
    }

    public async Task<bool> SendMessageAsync(string chatRoomId, string userId, string content)
    {
        var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
        if (chatRoom == null)
            return false;

        var message = new MessageModel
        {
            Id = Guid.NewGuid().ToString(),
            UserId = userId,
            Content = content,
            Timestamp = DateTime.UtcNow
        };

        var update = Builders<ChatRoomModel>.Update.Push("Messages", message);
        await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRoomId, update);

        return true;
    }

    public async Task<List<MessageModel>?> GetMessagesInChatRoomAsync(string chatRoomId)
    {
        var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
        return chatRoom?.Messages;
    }

    public async Task<bool> DeleteMessageAsync(string chatRoomId, string messageId)
    {
        var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
        if (chatRoom == null)
            return false;

        var update =
            Builders<ChatRoomModel>.Update.PullFilter("Messages",
                Builders<MessageModel>.Filter.Eq(m => m.Id, messageId));
        await _chatRooms.UpdateOneAsync(cr => cr.Id == chatRoomId, update);
        return true;
    }

    public async Task<List<MessageModel>> SearchMessagesInChatRoomAsync(string chatRoomId, string keyword)
    {
        var chatRoom = await _chatRooms.Find(cr => cr.Id == chatRoomId).FirstOrDefaultAsync();
        return chatRoom?.Messages?.Where(m => m.Content.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            .ToList() ?? new List<MessageModel>();
    }
}