namespace BinanceCryptoPriceAPI.Controllers
{
    #region Usings
    using Application.Interfaces;
    using BinanceCryptoPriceAPI.Dtos;
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
        public async Task<IActionResult> Get24hAvgPrice(string symbol)
        {
            var averagePrice = await _priceService.Get24hAvgPrice(symbol);

            var result = new PriceResult { Symbol = symbol, AveragePrice = averagePrice };

            return Ok(result);
        }

        [HttpGet("{symbol}/SimpleMovingAverage")]
        public async Task<IActionResult> GetSimpleMovingAverage(
                                                                string symbol,
                                                                [BindRequired, FromQuery(Name = "n")] int numberOfDataPoints,
                                                                [BindRequired, FromQuery(Name = "p")] string timePeriod,
                                                                [FromQuery(Name = "s")] DateTime? startDateTime)
        {
            var sma = await _priceService.GetSimpleMovingAverage(symbol, numberOfDataPoints, timePeriod, startDateTime);

            var smaResult = new SMAResult { Symbol = symbol, SMAAveragePrice = sma };
            return Ok(smaResult);
        }
        #endregion
    }
}
