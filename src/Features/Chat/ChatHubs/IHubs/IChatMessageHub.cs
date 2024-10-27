namespace HUBT_Social_API.Features.Chat.ChatHubs.IHubs;

public interface IChatMessageHub
{
    Task SendMessage(string chatRoomId, string userId, string messageContent);
    Task JoinRoom(string chatRoomId);
    Task LeaveRoom(string chatRoomId);
}