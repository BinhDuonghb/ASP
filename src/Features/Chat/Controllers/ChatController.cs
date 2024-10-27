using HUBT_Social_API.Features.Chat.DTOs;
using HUBT_Social_API.Features.Chat.Services.IChatServices;
using HUBTSOCIAL.Src.Features.Chat.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;

namespace HUBT_Social_API.Features.Chat.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ChatController : ControllerBase
{
    private readonly IChatService _chatService;
    private readonly IStringLocalizer<ChatController> _localizer;

    public ChatController(IChatService chatService, IStringLocalizer<ChatController> localizer)
    {
        _chatService = chatService;
        _localizer = localizer;
    }

    [HttpPost("send-message")]
    public async Task<IActionResult> SendMessage([FromBody] MessageDTO messageDto)
    {
        if (messageDto == null || string.IsNullOrEmpty(messageDto.Content) ||
            string.IsNullOrEmpty(messageDto.ChatRoomId))
            return BadRequest(new ChatResponseDTO<object>(false, _localizer["InvalidMessageData"]));

        var result = await _chatService.SendMessageAsync(messageDto);
        return result
            ? Ok(new ChatResponseDTO<object>(true, _localizer["MessageSentSuccessfully"]))
            : BadRequest(new ChatResponseDTO<object>(false, _localizer["FailedToSendMessage"]));
    }

    [HttpGet("{chatRoomId}/messages")]
    public async Task<IActionResult> GetMessages(string chatRoomId)
    {
        if (string.IsNullOrEmpty(chatRoomId))
            return BadRequest(new ChatResponseDTO<object>(false, _localizer["ChatRoomIdRequired"]));

        var messages = await _chatService.GetMessagesInChatRoomAsync(chatRoomId);
        return messages == null || messages.Count == 0
            ? NotFound(new ChatResponseDTO<object>(false, _localizer["NoMessagesFound"]))
            : Ok(new ChatResponseDTO<List<MessageModel>>(true, _localizer["MessagesFoundSuccessfully"], messages));
    }

    [HttpDelete("{chatRoomId}/messages/{messageId}")]
    public async Task<IActionResult> DeleteMessage(string chatRoomId, string messageId)
    {
        if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(messageId))
            return BadRequest(new ChatResponseDTO<object>(false, _localizer["ChatRoomIdAndKeywordRequired"]));

        var result = await _chatService.DeleteMessageAsync(chatRoomId, messageId);
        return result
            ? Ok(new ChatResponseDTO<object>(true, _localizer["MessageDeletedSuccessfully"]))
            : BadRequest(new ChatResponseDTO<object>(false, _localizer["FailedToDeleteMessage"]));
    }

    [HttpGet("{chatRoomId}/search")]
    public async Task<IActionResult> SearchMessages(string chatRoomId, [FromQuery] string keyword)
    {
        if (string.IsNullOrEmpty(chatRoomId) || string.IsNullOrEmpty(keyword))
            return BadRequest(new ChatResponseDTO<object>(false, _localizer["ChatRoomIdAndKeywordRequired"]));

        var messages = await _chatService.SearchMessagesInChatRoomAsync(chatRoomId, keyword);
        return messages == null || messages.Count == 0
            ? NotFound(new ChatResponseDTO<object>(false, _localizer["NoMessagesFound"]))
            : Ok(new ChatResponseDTO<List<MessageModel>>(true, _localizer["MessagesFoundSuccessfully"], messages));
    }

    [HttpPost("{chatRoomId}/upload-image")]
    public async Task<IActionResult> UploadImage(string userId, string chatRoomId, [FromForm] byte[] imageData)
    {
        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(chatRoomId) || imageData == null ||
            imageData.Length == 0)
            return BadRequest(new ChatResponseDTO<object>(false, _localizer["InvalidImageData"]));

        var result = await _chatService.UploadImageAsync(userId, chatRoomId, imageData);
        return result
            ? Ok(new ChatResponseDTO<object>(true, _localizer["ImageUploadedSuccessfully"]))
            : BadRequest(new ChatResponseDTO<object>(false, _localizer["FailedToUploadImage"]));
    }

    [HttpPost("{chatRoomId}/upload-file")]
    public async Task<IActionResult> UploadFile(string chatRoomId, [FromForm] byte[] fileData,
        [FromForm] string fileName)
    {
        if (string.IsNullOrEmpty(chatRoomId) || fileData == null || fileData.Length == 0 ||
            string.IsNullOrEmpty(fileName))
            return BadRequest(new ChatResponseDTO<object>(false, _localizer["InvalidFileData"]));

        var result = await _chatService.UploadFileAsync(chatRoomId, fileData, fileName);
        return result
            ? Ok(new ChatResponseDTO<object>(true, _localizer["FileUploadedSuccessfully"]))
            : BadRequest(new ChatResponseDTO<object>(false, _localizer["FailedToUploadFile"]));
    }
}