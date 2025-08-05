using ERPApp.Shared.Abstractions.Cryptography;
using System.Security.Cryptography;
using System.Text;

namespace ERPApp.Shared.Services.Cryptography;

public class PasswordHasherService : IPasswordHasherService
{
    public string Sha256Hash(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}
