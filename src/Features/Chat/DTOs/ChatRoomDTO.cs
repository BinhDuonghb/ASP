namespace HUBT_Social_API.Features.Chat.DTOs;

public class ChatRoomDTO
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string>? UserIds { get; set; } = new();
    public List<MessageDTO>? Messages { get; set; } = new();
}