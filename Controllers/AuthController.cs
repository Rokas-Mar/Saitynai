using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AppDbContext _db;
    private readonly ITokenService _tokenService;

    private const string RefreshCookieName = "refreshToken";

    public AuthController(AppDbContext db, ITokenService tokenService)
    {
        _db = db;
        _tokenService = tokenService;
    }

    public class LoginRequest
    {
        public string Email { get; set; } = default!;
        public string Password { get; set; } = default!;
    }

    public class LoginResponse
    {
        public string AccessToken { get; set; } = default!;
        public string Role { get; set; } = default!;
        public string Name { get; set; } = default!;
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _db.Users
            .SingleOrDefaultAsync(u => u.Email == request.Email && u.Password == request.Password);

        if (user == null)
            return Unauthorized("Invalid credentials");

        var accessToken = _tokenService.GenerateAccessToken(user);

        var refresh = _tokenService.CreateRefreshToken(user);
        _db.RefreshTokens.Add(refresh);
        await _db.SaveChangesAsync();

        _tokenService.SetRefreshTokenCookie(refresh.Token, refresh.ExpiresAt);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken,
            Role = user.Role,
            Name = $"{user.Name} {user.Surname}"
        });
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    public async Task<IActionResult> Refresh()
    {
        if (!Request.Cookies.TryGetValue(RefreshCookieName, out var tokenValue) ||
            string.IsNullOrWhiteSpace(tokenValue))
        {
            return Unauthorized("No refresh token");
        }

        var existing = await _db.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == tokenValue);

        if (existing == null || existing.IsRevoked || existing.ExpiresAt <= DateTime.UtcNow)
        {
            return Unauthorized("Invalid or expired refresh token");
        }

        var user = await _db.Users.FindAsync(existing.UserId);
        if (user == null)
            return Unauthorized("User not found");

        // revoke old token
        existing.IsRevoked = true;

        // issue new ones
        var newAccess = _tokenService.GenerateAccessToken(user);
        var newRefresh = _tokenService.CreateRefreshToken(user);
        _db.RefreshTokens.Add(newRefresh);

        await _db.SaveChangesAsync();

        _tokenService.SetRefreshTokenCookie(newRefresh.Token, newRefresh.ExpiresAt);

        return Ok(new { accessToken = newAccess });
    }

    // POST: api/Auth/logout
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
        if (Request.Cookies.TryGetValue(RefreshCookieName, out var tokenValue))
        {
            var existing = await _db.RefreshTokens
                .FirstOrDefaultAsync(r => r.Token == tokenValue);

            if (existing != null)
            {
                existing.IsRevoked = true;
                await _db.SaveChangesAsync();
            }

            Response.Cookies.Append(RefreshCookieName, string.Empty, new CookieOptions
            {
                Expires = DateTime.UtcNow.AddDays(-1),
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Lax,
                Path = "/"
            });
        }

        return Ok();
    }
}
