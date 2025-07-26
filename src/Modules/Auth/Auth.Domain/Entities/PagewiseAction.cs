namespace Auth.Domain.Entities;

public class PagewiseAction
{
    public string ActionID { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public int MenuId { get; set; }
    public string? ActionPermissionJson { get; set; }
}
