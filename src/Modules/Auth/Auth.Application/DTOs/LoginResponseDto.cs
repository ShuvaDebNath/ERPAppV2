namespace Auth.Application.DTOs;

public class LoginResponseDto
{
    public string Token { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string UserName { get; set; } = default!;
    public string? FullName { get; set; }
    public string? Role { get; set; }
    public string? Department { get; set; }
}
