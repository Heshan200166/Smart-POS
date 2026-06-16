using SmartPOS.Models;

namespace SmartPOS.Services;

/// <summary>
/// Interface for authentication services
/// </summary>
public interface IAuthenticationService
{
    Task<User?> LoginAsync(string username, string password);
    Task<bool> LogoutAsync(int userId);
    Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
    string HashPassword(string password);
    bool VerifyPassword(string password, string passwordHash);
}
