using System.Security.Claims;
using KNQARecruitmentPortal.Services;
using Microsoft.AspNetCore.Components.Authorization;

namespace KNQARecruitmentPortal.Authentication;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly IAuthenticationService _authService;
    private ClaimsPrincipal _cachedPrincipal = new(new ClaimsIdentity());
    private bool _isAuthenticated = false;

    public CustomAuthenticationStateProvider(IAuthenticationService authService)
    {
        _authService = authService;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        if (_isAuthenticated)
        {
            return new AuthenticationState(_cachedPrincipal);
        }

        return new AuthenticationState(_cachedPrincipal);
    }

    public void SetAuthenticationState(AuthenticationResult? result)
    {
        if (result?.IsSuccessful == true)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, result.UserId.ToString()),
                new Claim(ClaimTypes.Name, result.UserName),
                new Claim(ClaimTypes.Email, result.Email),
                new Claim(ClaimTypes.Role, result.RoleName),
                new Claim("FullName", result.FullName),
                new Claim("RoleId", result.RoleId.ToString())
            };

            var identity = new ClaimsIdentity(claims, "CustomAuth");
            _cachedPrincipal = new ClaimsPrincipal(identity);
            _isAuthenticated = true;
        }
        else
        {
            _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
            _isAuthenticated = false;
        }

        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public void ClearAuthenticationState()
    {
        _cachedPrincipal = new ClaimsPrincipal(new ClaimsIdentity());
        _isAuthenticated = false;
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public bool IsAuthenticated => _isAuthenticated;
}
