namespace BinanceCryptoPriceAPI.Controllers
{
    #region Usings
    using BinanceCryptoPriceAPI.Infrastructure.JWT;
    using Microsoft.AspNetCore.Mvc;
    using Models;
    #endregion

    [ApiController]
    [Route("api/[controller]")]
    public class TokenController : ControllerBase
    {
        private readonly IJwtToken _jwtToken;
        private readonly ILogger<TokenController> _logger;

        public TokenController(
            IJwtToken jwtToken,
            ILogger<TokenController> logger)
        {
            _jwtToken = jwtToken;
            _logger = logger;
        }


        [HttpPost("generateJwtToken")]
        public async Task<IActionResult> GenerateJWTToken()
        {
            try
            {
                var token = await _jwtToken.GenerateJwtToken();

                if (string.IsNullOrWhiteSpace(token))
                {
                    return BadRequest(Result<string>.Failure("Token is null or empty!"));
                }

                var tokenResult = new JwtTokenResult() { Token = token };

                var result = Result<JwtTokenResult>.Success(tokenResult);

                return Ok(result);
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(Result<string>.Failure(e.Message));
            }
        }
    }
}
