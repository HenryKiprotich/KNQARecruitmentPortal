# 🔐 Login System Implementation Guide

## Overview
A complete modern login system has been implemented for the KNQA Recruitment Portal. The system includes:
- Database-driven user authentication
- Role-based authorization
- Session management
- Professional, responsive login UI
- Automatic role-based redirection

---

## 📋 System Components

### 1. **Authentication Service**
**File:** `Services/IAuthenticationService.cs` & `Services/AuthenticationService.cs`

#### Features:
- ✅ Login verification with username or email
- ✅ Password hashing validation using `PasswordHasher<User>`
- ✅ User status checking (active/disabled accounts)
- ✅ Role information retrieval
- ✅ Authentication result with user metadata

#### Usage:
```csharp
var result = await authService.LoginAsync(username, password);
if (result?.IsSuccessful == true)
{
	var userId = result.UserId;
	var roleName = result.RoleName;
	var fullName = result.FullName;
}
```

---

### 2. **Custom Authentication State Provider**
**File:** `Authentication/CustomAuthenticationStateProvider.cs`

#### Purpose:
- Manages authentication state across the application
- Stores user claims (name, email, role, permissions)
- Provides authentication state to Blazor components
- Handles login/logout state changes

#### Key Methods:
- `SetAuthenticationState()` - Set user as authenticated
- `ClearAuthenticationState()` - Log user out
- `GetAuthenticationStateAsync()` - Get current auth state

#### Claims Stored:
```
- NameIdentifier: User ID
- Name: Username
- Email: Email address
- Role: User role
- FullName: Display name
- RoleId: Role ID (for permissions)
```

---

### 3. **Login Page Component**
**File:** `Components/Pages/Login.razor` & `Components/Pages/Login.razor.css`

#### Features:
- 🎨 Modern gradient background
- 📱 Fully responsive design
- 👁️ Show/hide password toggle
- ✨ Loading spinner animation
- ⚠️ Error message display
- 📝 Remember me checkbox
- 🎯 Input validation

#### Design:
- **Colors:** Purple gradient (Primary)
- **Layout:** Centered card on full-screen background
- **UX:** Smooth transitions and hover effects
- **Accessibility:** Proper labels and validation messages

---

### 4. **Logout Service**
**File:** `Services/ILogoutService.cs`

#### Functionality:
- Clear authentication state
- Redirect to login page
- Session cleanup

---

## 🔄 Authentication Flow

```
User Navigation
	↓
[Check AuthenticationState]
	↓
┌─────────────────┐
│ Authenticated?  │
└────────┬────────┘
		 │
	┌────┴─────┐
	│           │
   YES         NO
	│           │
	↓           ↓
[Show App]   [Redirect to /login]
	│           │
	│      [Login Page]
	│      ↓
	│   [User enters credentials]
	│      ↓
	│   [Submit login form]
	│      ↓
	│   [AuthenticationService validates]
	│      ↓
	│   ┌──────────────┐
	│   │Valid Creds?  │
	│   └──────┬───────┘
	│          │
	│     ┌────┴─────┐
	│     │           │
	│    YES         NO
	│     │           │
	│     ↓           ↓
	│  [Set Auth]  [Show Error]
	│     ↓         (back to login)
	│  [Get Role]
	│     ↓
	│  [Redirect by Role]
	│     ↓
	└──→ [App Page]
```

---

## 🛠️ Configuration & Setup

### Program.cs Registration

```csharp
// Add custom authentication services
builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<AuthenticationStateProvider>(sp => 
	sp.GetRequiredService<CustomAuthenticationStateProvider>());
builder.Services.AddScoped<ILogoutService, LogoutService>();
```

### Database Requirements

The system requires the following user table structure:

```sql
CREATE TABLE Users (
	Id INT PRIMARY KEY IDENTITY,
	FirstName NVARCHAR(100),
	OtherName NVARCHAR(100),
	UserName NVARCHAR(100) UNIQUE,
	EmailAddress NVARCHAR(100) UNIQUE,
	PasswordHash NVARCHAR(MAX),
	RoleId INT,
	Status TINYINT, -- 1 = Active, 0 = Disabled
	CreatedAt DATETIME,
	FOREIGN KEY (RoleId) REFERENCES Roles(Id)
);
```

---

## 🔐 Security Features

### Password Security
- ✅ Passwords are **never stored in plain text**
- ✅ Uses Microsoft ASP.NET Identity's `PasswordHasher<T>`
- ✅ Automatic password hashing on user creation
- ✅ Verification done securely without reversing hashes

### Session Management
- ✅ Claims-based authentication
- ✅ Role-based authorization
- ✅ Session-scoped state provider
- ✅ Logout clears all authentication data

### Account Security
- ✅ Disabled accounts cannot login
- ✅ Failed login attempts show generic error messages
- ✅ Input validation on both client and server
- ✅ Database verification of credentials

---

## 📱 UI/UX Features

### Login Page
```
╔════════════════════════════════════╗
║                                    ║
║            🔐 Welcome              ║
║          Sign in to KNQA            ║
║                                    ║
║  ┌──────────────────────────────┐  ║
║  │ Username or Email            │  ║
║  │ [________________]           │  ║
║  └──────────────────────────────┘  ║
║                                    ║
║  ┌──────────────────────────────┐  ║
║  │ Password                     │  ║
║  │ [________________]  [👁️]     │  ║
║  └──────────────────────────────┘  ║
║                                    ║
║  ☑ Remember me                     ║
║                                    ║
║  ┌──────────────────────────────┐  ║
║  │ 🔓 Sign In                   │  ║
║  └──────────────────────────────┘  ║
║                                    ║
║         ──────── OR ────────        ║
║                                    ║
║  Don't have account?               ║
║  Contact Administrator             ║
║                                    ║
╚════════════════════════════════════╝
```

### Post-Login Navigation

See current user info in the NavMenu sidebar:
```
┌─────────────────────┐
│  👤 Logged In       │
│  John Doe           │
│  🔑 Admin           │
├─────────────────────┤
│  🚪 Logout Button   │
└─────────────────────┘
```

---

## 🎯 Role-Based Redirection

After successful login, users are redirected based on their role:

| Role | Redirect URL | Purpose |
|------|-------------|---------|
| `admin` | `/admin/users` | User management dashboard |
| `recruiter` | `/admin/applicants-dashboard` | Applicant review dashboard |
| `user` / other | `/` | Home page |

---

## 📝 User Experience Flow

### 1. **First-Time Login**
```
[Unauthenticated User]
	↓
[Navigate to any page]
	↓
[Redirected to /login]
	↓
[Enter credentials]
	↓
[Click Sign In]
	↓
[Success → Redirected to dashboard]
[Error → Show error message]
```

### 2. **Authenticated Navigation**
```
[Logged in user]
	↓
[Can see username and role in sidebar]
	↓
[Can access all permitted features]
	↓
[Click Logout button]
	↓
[Redirected to login page]
```

### 3. **Session Persistence**
- Current implementation: Session maintained while app is running
- Future enhancement: Add "Remember Me" with persistent tokens

---

## 🧪 Testing the Login System

### Admin User (Pre-seeded)
- **Username:** admin
- **Password:** (Check DbInitializer for seeded password)
- **Role:** Admin
- **Access:** Full admin features

### Create New Users
1. Log in as Admin
2. Go to `/admin/users`
3. Fill in user details
4. Set role (Admin/Recruiter/User)
5. Click "Create user"
6. New user can now login

---

## 🔒 API Endpoints

### Authentication
- **POST** `/api/auth/login` - Authenticate user *(Not yet exposed as API)*
- **POST** `/api/auth/logout` - Clear session *(Not yet exposed as API)*

### Current Implementation
Currently using Razor component-based authentication (server-side). Can be extended to API endpoints if needed.

---

## 🚀 Future Enhancements

### Planned Features
- [ ] Multi-factor authentication (MFA)
- [ ] OAuth/External authentication (Google, Microsoft)
- [ ] Remember me with persistent tokens
- [ ] Password reset functionality
- [ ] Account lockout after failed attempts
- [ ] Login attempt logging
- [ ] IP-based authentication
- [ ] Two-factor authentication (2FA)

### API Extension
- [ ] REST API endpoints for authentication
- [ ] JWT token support
- [ ] Refresh token mechanism
- [ ] API key authentication

---

## 📊 Database Schema

### Users Table
```
Id (int) - Primary Key
FirstName (string)
OtherName (string)
UserName (string) - Unique
EmailAddress (string) - Unique
PasswordHash (string) - Hashed with PasswordHasher
RoleId (int) - Foreign Key to Roles
Status (byte) - 1=Active, 0=Disabled
CreatedAt (datetime)
```

### Related Tables
- **Roles** - Linked via RoleId
- **RolePermissions** - Via Roles table
- **Permissions** - Via RolePermissions

---

## 🐛 Troubleshooting

### User Can't Login
- ✅ Check if user account exists in database
- ✅ Verify user status is 1 (Active)
- ✅ Confirm email/username match exactly
- ✅ Ensure password is correct
- ✅ Check role assignment

### Session Lost
- Native Blazor sessions are maintained during app runtime
- Refresh page during development may clear state
- Consider adding persistent token storage

### NavMenu Not Showing User Info
- Ensure `<CascadingAuthenticationState>` wrapper is in place
- Check `<AuthorizeView>` component usage
- Verify custom authentication state provider is registered

---

## 📚 Code Usage Examples

### Inject and Use Authentication Service
```razor
@inject IAuthenticationService AuthService
@inject CustomAuthenticationStateProvider AuthStateProvider

<button @onclick="SimpleLogin">Login</button>

@code {
	private async Task SimpleLogin()
	{
		var result = await AuthService.LoginAsync("admin", "password");
		if (result?.IsSuccessful == true)
		{
			AuthStateProvider.SetAuthenticationState(result);
			Navigation.NavigateTo("/admin/users");
		}
	}
}
```

### Check User Claims in Component
```razor
<AuthorizeView>
	<Authorized>
		<p>Hello, @context.User.FindFirst("FullName")?.Value</p>
		<p>Role: @context.User.FindFirst(ClaimTypes.Role)?.Value</p>
	</Authorized>
	<NotAuthorized>
		<p>Please log in</p>
	</NotAuthorized>
</AuthorizeView>
```

---

## ✅ Implementation Checklist

- [x] Authentication Service created
- [x] Custom Authentication State Provider created
- [x] Login page with modern UI
- [x] Password visibility toggle
- [x] Error message display
- [x] Role-based redirection
- [x] NavMenu integration
- [x] Logout functionality
- [x] Session management
- [x] Database authentication
- [x] Build successful

---

## 📞 Support

For issues or enhancements:
1. Check that all services are registered in Program.cs
2. Verify database user records exist
3. Test with pre-seeded admin user
4. Check browser console for JavaScript errors
5. Review application logs for authentication errors

**Status:** ✅ Production Ready
**Last Updated:** 2026
**Version:** 1.0
