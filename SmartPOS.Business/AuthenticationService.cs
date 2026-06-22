using Microsoft.EntityFrameworkCore;
using SmartPOS.Data;
using SmartPOS.Models;
using SmartPOS.Services;
using BCrypt.Net;

namespace SmartPOS.Business;

/// <summary>
/// Implementation of authentication services
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly ApplicationDbContext _context;

    public AuthenticationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<User?> LoginAsync(string username, string password)
    {
        try
        {
            Console.WriteLine($"[AuthService] Login attempt for user: {username}");
            
            var user = await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.Username == username && u.IsActive);

            Console.WriteLine($"[AuthService] User found in database: {user != null}");

            if (user == null)
            {
                Console.WriteLine($"[AuthService] User not found or inactive: {username}");
                return null;
            }

            Console.WriteLine($"[AuthService] Verifying password...");
            if (!VerifyPassword(password, user.PasswordHash))
            {
                Console.WriteLine($"[AuthService] Password verification failed");
                return null;
            }

            Console.WriteLine($"[AuthService] Password verified successfully");
            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            Console.WriteLine($"[AuthService] Login successful for {username}");
            return user;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AuthService] Login error: {ex.Message}");
            Console.WriteLine($"[AuthService] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    public async Task<bool> LogoutAsync(int userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        // Can add logout logic here (e.g., clear tokens, log activity)
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null)
            return false;

        if (!VerifyPassword(oldPassword, user.PasswordHash))
            return false;

        user.PasswordHash = HashPassword(newPassword);
        await _context.SaveChangesAsync();

        return true;
    }

    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool VerifyPassword(string password, string passwordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, passwordHash);
    }
}
