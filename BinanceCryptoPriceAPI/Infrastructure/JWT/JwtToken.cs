namespace BinanceCryptoPriceAPI.Infrastructure.JWT
{
    #region Usings
    using Microsoft.IdentityModel.Tokens;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    #endregion

    public class JwtToken : IJwtToken
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<JwtToken> _logger;

        public JwtToken(
            IConfiguration configuration,
            ILogger<JwtToken> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<string> GenerateJwtToken()
        {
            var jwt = string.Empty;

            try
            {
                List<Claim> claims = new List<Claim>()
                {
                    new Claim(ClaimTypes.Authentication, Guid.NewGuid().ToString()),
                    new Claim(ClaimTypes.NameIdentifier, "Binance-Crypto-Price-App")
                };

                var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_configuration.GetSection("Jwt:Secret").Value));
                var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature);

                var tokenData = new JwtSecurityToken(
                claims: claims,
                    expires: DateTime.Now.AddMinutes(int.Parse(_configuration.GetSection("Jwt:DurationMinutes").Value)),
                    issuer: _configuration.GetSection("Jwt:Issuer").Value,
                    signingCredentials: credentials
                );

                jwt = new JwtSecurityTokenHandler().WriteToken(tokenData);
            }
            catch (Exception)
            {
                throw;
            }

            return jwt;
        }
    }
}
