using Auth.Application.DTOs;
using System.Threading.Tasks;

namespace Auth.Application.Interfaces;

public interface IUserPermissionService
{
    Task<UserPermissionDto> GetUserPermissionsAsync(string userId);
    Task<bool> HasPermissionAsync(string userId, int menuId, string actionType); // actionType = "View", "Insert", etc.
}
