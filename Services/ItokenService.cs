public interface ITokenService
{
    string GenerateAccessToken(User user);
    RefreshToken CreateRefreshToken(User user);
    void SetRefreshTokenCookie(string tokenValue, DateTime expiresAt);
}
