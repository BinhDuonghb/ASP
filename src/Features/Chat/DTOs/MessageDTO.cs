namespace HUBT_Social_API.Features.Chat.DTOs;

public class MessageDTO
{
    public string UserId { get; set; } = string.Empty;
    public string ChatRoomId { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
}