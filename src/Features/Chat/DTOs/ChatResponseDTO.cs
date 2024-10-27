namespace HUBT_Social_API.Features.Chat.DTOs;

public class ChatResponseDTO<T>
{
#pragma warning disable CS8601 // Possible null reference assignment.
    public ChatResponseDTO(bool success, string message, T data = default)
#pragma warning restore CS8601 // Possible null reference assignment.
    {
        Success = success;
        Message = message;
        Data = data;
    }

    public bool Success { get; set; }
    public string Message { get; set; }
    public T Data { get; set; }
}