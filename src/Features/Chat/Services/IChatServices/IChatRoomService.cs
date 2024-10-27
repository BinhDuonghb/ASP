using HUBTSOCIAL.Src.Features.Chat.Models;

namespace HUBT_Social_API.Features.Chat.Services.IChatServices;

public interface IChatRoomService
{
    Task<bool> CreateGroupAsync(string nameGroup);
    Task<bool> UpdateGroupNameAsync(string idGroup, string newGroupName);
    Task<bool> DeleteGroupAsync(string idGroup);
    Task<ChatRoomModel> GetGroupByIdAsync(string id);
    Task<List<ChatRoomModel>> GetAllGroupsAsync();
}