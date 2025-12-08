using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

public class TokenService : ITokenService
{
    private readonly JwtSettings _jwtSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IConfiguration _configuration;

    public TokenService(
        IOptions<JwtSettings> jwtOptions,
        IHttpContextAccessor httpContextAccessor,
        IConfiguration configuration)
    {
        _jwtSettings = jwtOptions.Value;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
    }

    public string GenerateAccessToken(User user)
    {
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, $"{user.Name} {user.Surname}"),
            new Claim(ClaimTypes.Role, user.Role),
            new Claim("organisationId", user.OrganisationId.ToString())
        };

        var expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpireMinutes);

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.Issuer,
            audience: _jwtSettings.Audience,
            claims: claims,
            expires: expires,
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public RefreshToken CreateRefreshToken(User user)
    {
        var value = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));

        return new RefreshToken
        {
            Token = value,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsRevoked = false
        };
    }

	public void SetRefreshTokenCookie(string tokenValue, DateTime expiresAt)
	{
		var context = _httpContextAccessor.HttpContext
			?? throw new InvalidOperationException("No active HttpContext");

		var opts = new CookieOptions
		{
			HttpOnly = true,
			Secure = true,
			SameSite = SameSiteMode.None,
			Expires = expiresAt,
			Path = "/"
		};

		context.Response.Cookies.Append("refreshToken", tokenValue, opts);
	}

}
