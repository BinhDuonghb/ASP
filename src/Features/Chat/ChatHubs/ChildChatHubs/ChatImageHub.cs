using HUBT_Social_API.Features.Chat.ChatHubs.IHubs;
using HUBT_Social_API.Features.Chat.Services.IChatServices;
using Microsoft.AspNetCore.SignalR;

namespace HUBT_Social_API.Features.Chat.ChatHubs.ChildChatHubs;

public class ChatImageHub : Hub, IChatImageHub
{
    private readonly IChatService _chatService;

    public ChatImageHub(IChatService chatService)
    {
        _chatService = chatService;
    }

    /// <summary>
    ///     Gửi hình ảnh đến tất cả người dùng trong phòng chat.
    /// </summary>
    public async Task SendImage(string chatRoomId, string userId, byte[] imageData)
    {
        // Giả sử bạn đã có phương thức UploadImageAsync trong IChatService
        var imageUrl = await _chatService.UploadImageAsync(userId, chatRoomId, imageData);
        if (imageUrl)
            await Clients.Group(chatRoomId).SendAsync("ReceiveImage", new { UserId = userId, ImageUrl = imageUrl });
    }
}