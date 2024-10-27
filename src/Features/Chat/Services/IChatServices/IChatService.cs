using HUBT_Social_API.Features.Chat.DTOs;
using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.Services.IChatServices;

public interface IChatService
{
    Task<bool> SendMessageAsync(MessageDTO messageDto);
    Task<List<MessageModel>?> GetMessagesInChatRoomAsync(string chatRoomId);
    Task<bool> DeleteMessageAsync(string chatRoomId, string messageId);
    Task<List<MessageModel>> SearchMessagesInChatRoomAsync(string chatRoomId, string keyword);
    Task<bool> UploadImageAsync(string userId, string chatRoomId, byte[] imageData);
    Task<bool> UploadFileAsync(string chatRoomId, byte[] fileData, string fileName);
}