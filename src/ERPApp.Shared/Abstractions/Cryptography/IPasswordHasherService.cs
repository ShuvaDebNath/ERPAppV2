namespace ERPApp.Shared.Abstractions.Cryptography;

public interface IPasswordHasherService
{
    string Sha256Hash(string password);
}
