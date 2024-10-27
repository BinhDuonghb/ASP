namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class ChatRoomModel
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public List<string>? UserIds { get; set; } = new();
    public List<MessageModel>? Messages { get; set; } = new();
    public DateTime CreatedAt { get; set; }
}