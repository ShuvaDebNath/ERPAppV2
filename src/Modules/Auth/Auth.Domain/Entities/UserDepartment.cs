namespace Auth.Domain.Entities;

public class UserDepartment
{
    public string DeptId { get; set; } = default!;
    public string? DeptName { get; set; }
    public int IsActive { get; set; }

    // Navigation (optional)
    public List<UserControl>? Users { get; set; }
}
