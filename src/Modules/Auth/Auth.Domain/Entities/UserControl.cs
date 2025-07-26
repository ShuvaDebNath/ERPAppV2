namespace Auth.Domain.Entities;

public class UserControl
{
    public string UserId { get; set; } = default!;
    public string Id { get; set; } = default!;
    public string? FullName { get; set; }
    public string UserTypeId { get; set; } = default!;
    public string? MenuId { get; set; }
    public string? MakeBy { get; set; }
    public DateTime? MakeDate { get; set; }
    public string? UpdateBy { get; set; }
    public DateTime? UpdateDate { get; set; }
    public string? DeleteBy { get; set; }
    public DateTime? DeleteDate { get; set; }
    public bool? IsActive { get; set; }
    public bool? DashboardPreview { get; set; }
    public string? UserRoleID { get; set; }
    public string? CountryType { get; set; }
    public string? UserType { get; set; }
    public string? DeptId { get; set; } 
    public UserDepartment? Department { get; set; }
    public UserRole? Role { get; set; }
    public UserType? Type { get; set; }
}
