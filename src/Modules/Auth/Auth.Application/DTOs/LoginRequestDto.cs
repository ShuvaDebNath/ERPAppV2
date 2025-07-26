using System.ComponentModel.DataAnnotations;

namespace Auth.Application.DTOs;

public class LoginRequestDto
{
    [Required(ErrorMessage = "Username is required")]
    public string UserName { get; set; } = default!;

    [Required(ErrorMessage = "Password is required")]
    public string Password { get; set; } = default!;
}
