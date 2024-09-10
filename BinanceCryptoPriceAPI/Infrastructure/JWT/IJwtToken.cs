namespace BinanceCryptoPriceAPI.Infrastructure.JWT
{
    public interface IJwtToken
    {
        Task<string> GenerateJwtToken();
    }
}
