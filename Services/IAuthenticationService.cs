namespace KNQARecruitmentPortal.Services;

public interface IAuthenticationService
{
    Task<AuthenticationResult?> LoginAsync(string username, string password);
    Task LogoutAsync();
    Task<AuthenticationResult?> GetCurrentUserAsync();
}

public class AuthenticationResult
{
    public int UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string RoleName { get; set; } = string.Empty;
    public int RoleId { get; set; }
    public bool IsSuccessful { get; set; }
    public string? ErrorMessage { get; set; }
}
