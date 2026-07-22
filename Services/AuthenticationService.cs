using KNQARecruitmentPortal.Data;
using KNQARecruitmentPortal.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace KNQARecruitmentPortal.Services;

public class AuthenticationService(AppDbContext db) : IAuthenticationService
{
    private readonly AppDbContext _db = db;

    public async Task<AuthenticationResult?> LoginAsync(string username, string password)
    {
        // Find user by username or email
        var user = await _db.Users
            .Include(u => u.Role)
            .FirstOrDefaultAsync(u => u.UserName == username || u.EmailAddress == username);

        if (user == null)
        {
            return new AuthenticationResult
            {
                IsSuccessful = false,
                ErrorMessage = "Invalid username or email."
            };
        }

        // Verify password
        var hasher = new PasswordHasher<User>();
        var result = hasher.VerifyHashedPassword(user, user.PasswordHash, password);

        if (result == PasswordVerificationResult.Failed)
        {
            return new AuthenticationResult
            {
                IsSuccessful = false,
                ErrorMessage = "Invalid password."
            };
        }

        // Check if user is active
        if (user.Status == 0)
        {
            return new AuthenticationResult
            {
                IsSuccessful = false,
                ErrorMessage = "User account is disabled. Contact administrator."
            };
        }

        // Return success with user data
        return new AuthenticationResult
        {
            IsSuccessful = true,
            UserId = user.Id,
            UserName = user.UserName,
            FullName = $"{user.FirstName} {user.OtherName}".Trim(),
            Email = user.EmailAddress,
            RoleId = user.RoleId,
            RoleName = user.Role?.RoleName ?? "User"
        };
    }

    public async Task LogoutAsync()
    {
        // This will be handled by AuthenticationStateProvider
        // We can add custom logic here if needed
        await Task.CompletedTask;
    }

    public async Task<AuthenticationResult?> GetCurrentUserAsync()
    {
        // This will be implemented in the authentication state provider
        return await Task.FromResult<AuthenticationResult?>(null);
    }
}
