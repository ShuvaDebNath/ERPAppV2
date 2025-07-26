namespace Auth.Domain.Entities;

public class Menu
{
    public int MenuId { get; set; }
    public string? MenuName { get; set; }
    public string? SubMenuName { get; set; }
    public string? UiLink { get; set; }
    public bool? IsActive { get; set; }
    public bool? YsnParent { get; set; }
    public int? OrderBy { get; set; }
    public DateTime? MakeDate { get; set; }
    public string? MenuLogo { get; set; }
}
