namespace BinanceCryptoPriceAPI.Controllers
{
    #region Usings
    using Application.Services;
    using Microsoft.AspNetCore.Mvc;
    #endregion

    [ApiController]
    [Route("api/[controller]")]
    public class WebSocketController : ControllerBase
    {
        private readonly BinanceWebSocketService _webSocketService;

        public WebSocketController(BinanceWebSocketService webSocketService)
        {
            _webSocketService = webSocketService;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartWebSocket()
        {
            await _webSocketService.StartAsync(CancellationToken.None);
            return Ok("WebSocket started.");
        }

        [HttpPost("stop")]
        public IActionResult StopWebSocket()
        {
            _webSocketService.Stop();
            return Ok("WebSocket stopped.");
        }

        [HttpPost("restart")]
        public async Task<IActionResult> RestartWebSocket()
        {
            await _webSocketService.RestartWebSocketAsync();
            return Ok("WebSocket restarted.");
        }
    }

}
