using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Auth.Infrastructure.Services;

public class UserPermissionService : IUserPermissionService
{
    private readonly AuthDbContext _context;

    public UserPermissionService(AuthDbContext context)
    {
        _context = context;
    }

    public async Task<UserPermissionDto> GetUserPermissionsAsync(string userId)
    {
        var result = new UserPermissionDto
        {
            UserId = userId
        };

        // 1. Get MenuIds from tblUserControl (comma-separated)
        var user = await _context.UserControls.FirstOrDefaultAsync(u => u.UserId == userId);

        if (user?.MenuId != null)
        {
            result.MenuIds = user.MenuId
                .Split(",", StringSplitOptions.RemoveEmptyEntries)
                .Select(int.Parse)
                .ToList();
        }

        // 2. Get ActionPermissions from tblPagewiseAction
        var pageActions = await _context.PagewiseActions
            .Where(p => p.UserId == userId)
            .ToListAsync();

        foreach (var action in pageActions)
        {
            var permission = ParseActionPermission(action.ActionPermissionJson);
            result.PagewisePermissions[action.MenuId] = permission;
        }

        return result;
    }

    public async Task<bool> HasPermissionAsync(string userId, int menuId, string actionType)
    {
        var pageAction = await _context.PagewiseActions
            .FirstOrDefaultAsync(p => p.UserId == userId && p.MenuId == menuId);

        if (pageAction == null || string.IsNullOrEmpty(pageAction.ActionPermissionJson))
            return false;

        var permissionDict = ParseActionPermission(pageAction.ActionPermissionJson);

        return permissionDict.TryGetValue(actionType, out var allowed) && allowed;
    }

    private Dictionary<string, bool> ParseActionPermission(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return new Dictionary<string, bool>();

        try
        {
            return JsonConvert.DeserializeObject<Dictionary<string, bool>>(json)
                ?? new Dictionary<string, bool>();
        }
        catch
        {
            return new Dictionary<string, bool>();
        }
    }
}
