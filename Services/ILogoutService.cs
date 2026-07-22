using KNQARecruitmentPortal.Authentication;

namespace KNQARecruitmentPortal.Services;

public interface ILogoutService
{
    Task LogoutAsync();
}

public class LogoutService(CustomAuthenticationStateProvider authStateProvider) : ILogoutService
{
    private readonly CustomAuthenticationStateProvider _authStateProvider = authStateProvider;

    public async Task LogoutAsync()
    {
        _authStateProvider.ClearAuthenticationState();
        await Task.CompletedTask;
    }
}
