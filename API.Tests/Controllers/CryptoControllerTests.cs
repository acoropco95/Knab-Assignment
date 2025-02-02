using Knab.Assignment.API.Controllers;
using Knab.Assignment.API.Models;
using Knab.Assignment.API.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace API.Tests.Controllers
{
    public class CryptoControllerTests
    {
        private readonly CryptoController _sut;
        private readonly ILogger<CryptoController> _logger;
        private readonly ICryptoService _cryptoService;

        public CryptoControllerTests()
        {
            _logger = Substitute.For<ILogger<CryptoController>>();
            _cryptoService = Substitute.For<ICryptoService>();

            _sut = new CryptoController(_logger, _cryptoService);
        }

        [Fact]
        public async Task GetQuote_WhenServiceReturnsQuote_ReturnQuote()
        {
            // Arrange
            var cryptoCurrency = "BTC";
            var currencies = new[] { "USD", "EUR" };
            var quote = new CryptoQuote
            {
                Base = "BTC",
                Rates = new Dictionary<string, decimal>
                {
                    ["USD"] = 50000,
                    ["EUR"] = 42000
                }
            };

            _cryptoService.GetQuote(cryptoCurrency, currencies).Returns(quote);

            // Act
            var result = await _sut.GetQuote(cryptoCurrency, currencies);

            // Assert
            Assert.Equal(typeof(OkObjectResult), result.Result!.GetType());
            Assert.Equal(quote, ((OkObjectResult)result.Result).Value);
        }

        [Fact]
        public async Task GetQuote_WhenServiceReturnsNull_ReturnBadRequest()
        {
            // Arrange
            var cryptoCurrency = "BTC";
            var currencies = new[] { "USD", "EUR" };

            _cryptoService.GetQuote(cryptoCurrency, currencies).Returns((CryptoQuote?)null);

            // Act
            var result = await _sut.GetQuote(cryptoCurrency, currencies);

            // Assert
            Assert.Equal(StatusCodes.Status400BadRequest, ((ObjectResult)result.Result!).StatusCode);
        }
    }
}
