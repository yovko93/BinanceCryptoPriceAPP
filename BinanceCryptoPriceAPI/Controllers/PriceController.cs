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
        [Produces("application/json", "application/xml")]
        public async Task<IActionResult> Get24hAvgPrice(string symbol)
        {
            var averagePrice = await _priceService.Get24hAvgPrice(symbol);

            var result = new PriceResult { Symbol = symbol, AveragePrice = averagePrice };

            return Ok(result);

            //if (Request.Headers["Accept"] == "application/xml")
            //{
            //    // Return XML response
            //    var priceResult = JsonConvert.SerializeObject(result);
            //    var xmlResult = JsonConvert.DeserializeXmlNode(priceResult, "PriceResult");
            //    return Ok(xmlResult);
            //}
            //else
            //{
            //    // return JSON response
            //    return Ok(result);
            //}
        }

        [HttpGet("{symbol}/SimpleMovingAverage")]
        public async Task<IActionResult> GetSimpleMovingAverage(
                                                                [BindRequired] string symbol,
                                                                [BindRequired, FromQuery(Name = "amount_of_data_points")] int n,
                                                                [BindRequired, FromQuery(Name = "time_period")] string p,
                                                                [FromQuery(Name = "start_date YYYY-MM-DD")] DateTime? s)
        {
            var sma = await _priceService.GetSimpleMovingAverage(symbol, n, p, s);

            return Ok();
        }
        #endregion
    }
}
