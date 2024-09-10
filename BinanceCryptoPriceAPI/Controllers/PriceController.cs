namespace BinanceCryptoPriceAPI.Controllers
{
    #region Usings
    using Application.Interfaces;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.ModelBinding;
    #endregion

    [ApiController]
    [Route("api")]
    public class PriceController : ControllerBase
    {
        private readonly ILogger<PriceController> _logger;
        private readonly IPriceService _priceService;

        public PriceController(
            ILogger<PriceController> logger,
            IPriceService priceService)
        {
            _logger = logger;
            _priceService = priceService;
        }

        #region Get
        [HttpGet("{symbol}/24hAvgPrice")]
        [Authorize]
        public async Task<IActionResult> Get24hAvgPrice(string symbol)
        {
            try
            {
                _logger.LogInformation($"Calculate 24h average price for {symbol}");
                var averagePriceResult = await _priceService.Get24hAvgPrice(symbol);

                if (averagePriceResult.IsSuccess)
                {
                    return Ok(averagePriceResult);
                }
                else
                {
                    _logger.LogError(averagePriceResult.Exception, averagePriceResult.ErrorMessage);
                    return BadRequest(averagePriceResult);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new { e.Message });
            }
        }

        [HttpGet("{symbol}/SimpleMovingAverage")]
        [Authorize]
        public async Task<IActionResult> GetSimpleMovingAverage(
                                                                string symbol,
                                                                [BindRequired, FromQuery(Name = "n")] int numberOfDataPoints,
                                                                [BindRequired, FromQuery(Name = "p")] string timePeriod,
                                                                [FromQuery(Name = "s")] DateTime? startDateTime)
        {
            try
            {
                _logger.LogInformation($"Calculate SMA for {symbol}");
                var smaResult = await _priceService.GetSimpleMovingAverage(symbol, numberOfDataPoints, timePeriod, startDateTime);

                if (smaResult.IsSuccess)
                {
                    return Ok(smaResult);
                }
                else
                {
                    _logger.LogError(smaResult.Exception, smaResult.ErrorMessage);
                    return BadRequest(smaResult);
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return BadRequest(new { e.Message });
            }
        }
        #endregion
    }
}
