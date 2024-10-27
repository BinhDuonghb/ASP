namespace HUBT_Social_API.Features.Chat.DTOs;

public class FileDTO
{
    /// <summary>
    ///     Tên file.
    /// </summary>
    public string FileName { get; set; } = string.Empty;

    /// <summary>
    ///     Đường dẫn hoặc URL đến file.
    /// </summary>
    public string FileUrl { get; set; } = string.Empty;

    /// <summary>
    ///     Loại file (ví dụ: image, video, document, audio, etc.).
    /// </summary>
    public string FileType { get; set; } = string.Empty;

    /// <summary>
    ///     Kích thước file tính theo byte.
    /// </summary>
    public long FileSize { get; set; }

    /// <summary>
    ///     Người dùng tải lên file.
    /// </summary>
    public string UploadedBy { get; set; } = string.Empty;

    /// <summary>
    ///     Ngày giờ file được tải lên.
    /// </summary>
    public DateTime UploadedAt { get; set; }

    /// <summary>
    ///     File dưới dạng byte array (nếu cần truyền tải nội dung file).
    /// </summary>
    public byte[]? FileContent { get; set; }
}