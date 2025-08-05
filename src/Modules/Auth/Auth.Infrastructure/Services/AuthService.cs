using Auth.Application.DTOs;
using Auth.Application.Interfaces;
using Auth.Domain.Entities;
using ERPApp.Shared.Abstractions.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Auth.Infrastructure.Services;

public class AuthService(
    AuthDbContext context, 
    IConfiguration config, 
    IPasswordHasher<AspNetUser> passwordHasher,
    IPasswordHasherService _hasher) : IAuthService
{
    private readonly AuthDbContext _context = context;
    private readonly IConfiguration _config = config;
    private readonly IPasswordHasher<AspNetUser> _passwordHasher = passwordHasher;
    private readonly IPasswordHasherService _hasher = _hasher;

    public async Task<LoginResponseDto?> LoginAsync(LoginRequestDto request)
    {
        var user = await _context.AspNetUsers
                                .Where(u => u.UserName == request.UserName)
                                .FirstOrDefaultAsync();

        if (user == null)
            return null;

        bool isPasswordValid = false;

        // 1. Try new password hashing (safe default)
        var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash ?? "", request.Password);
        if (result == PasswordVerificationResult.Success)
        {
            isPasswordValid = true;
        }
        else
        {
            // 2. Fallback to old SHA256
            var legacyHash = _hasher.Sha256Hash(request.Password);
            if (legacyHash == user.PasswordHash)
            {
                isPasswordValid = true;

                // 3. Upgrade to PasswordHasher format for future logins
                //user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
                //_context.AspNetUsers.Update(user);
                //await _context.SaveChangesAsync();
            }
        }

        if (!isPasswordValid)
            return null;

        var userControl = await _context.UserControls
            .FirstOrDefaultAsync(u => u.Id == user.Id);

        if (userControl == null || userControl.IsActive != true)
            return null;

        var dept = await _context.Departments
                         .FirstOrDefaultAsync(d => d.DeptId == userControl.DeptId);

        var isValidUser = await ValidateUserAsync(user.Id);
        if (!isValidUser)
            return null;

        if (dept == null)
            return null;

        var token = GenerateJwtToken(userControl);

        return new LoginResponseDto
        {
            Token = token,
            UserId = userControl.UserId,
            UserName = user.UserName,
            FullName = userControl.FullName,
            Role = userControl.UserRoleID,
            Department = dept.DeptId
        };
    }

    public async Task<bool> ValidateUserAsync(string userId)
    {
        var user = await _context.UserControls.FirstOrDefaultAsync(u => u.Id == userId);
        return user != null && user.IsActive == true;
    }

    private string GenerateJwtToken(UserControl user)
    {
        var claims = new[]
        {
        new Claim(ClaimTypes.NameIdentifier, user.UserId),
        new Claim(ClaimTypes.Name, user.FullName ?? user.UserId),
        new Claim(ClaimTypes.Role, user.UserRoleID ?? "")
    };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"],
            audience: _config["Jwt:Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(6),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

}
