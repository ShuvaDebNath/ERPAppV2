using System.Collections.Generic;

namespace Auth.Application.DTOs;

public class UserPermissionDto
{
    public string UserId { get; set; } = default!;
    public List<int> MenuIds { get; set; } = new();
    public Dictionary<int, Dictionary<string, bool>> PagewisePermissions { get; set; } = new();
}
