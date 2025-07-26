using Auth.Application.DTOs;
using System.Threading.Tasks;

namespace Auth.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto request);
    Task<bool> ValidateUserAsync(string userId);
}
