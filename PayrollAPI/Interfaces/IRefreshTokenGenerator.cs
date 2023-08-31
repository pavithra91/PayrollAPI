namespace PayrollAPI.Interfaces
{
    public interface IRefreshTokenGenerator
    {
        string GenerateToken(string username);
    }
}
