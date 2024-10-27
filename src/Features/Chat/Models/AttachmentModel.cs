namespace HUBTSOCIAL.Src.Features.Chat.Models;

public class AttachmentModel
{
    public string Id { get; set; } = string.Empty;
    public string MessageId { get; set; } = string.Empty;
    public string FileUrl { get; set; } = string.Empty;
    public string FileType { get; set; } = string.Empty; // Image, Video, etc.
    public long Size { get; set; }
    public DateTime UploadedAt { get; set; }
}