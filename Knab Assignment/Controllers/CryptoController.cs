using Knab.Assignment.API.Models;
using Knab.Assignment.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Knab.Assignment.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CryptoController : ControllerBase
    {
        private readonly ILogger<CryptoController> _logger;
        private readonly ICryptoService _cryptoService;

        public CryptoController(ILogger<CryptoController> logger, ICryptoService cryptoService)
        {
            _logger = logger;
            _cryptoService = cryptoService;
        }

        [HttpPost("quote")]
        public async Task<ActionResult<CryptoQuote?>> GetQuote([FromQuery]string cryptoCurrency, [FromBody] string[]? currencies)
        {
            currencies ??= ["USD", "EUR", "BRL", "GBP", "AUD"];

            try
            {
                var result = await _cryptoService.GetQuote(cryptoCurrency, currencies);

                if (result == null)
                {
                    return Problem(detail: $"No quote could not be retrieved.", statusCode: StatusCodes.Status400BadRequest);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while trying to get quote.");
                return Problem(detail: "Failed to obtain quote!", statusCode: StatusCodes.Status500InternalServerError);
            }
        }
    }
}
