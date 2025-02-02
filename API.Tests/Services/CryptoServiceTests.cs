using Knab.Assignment.API.Models;
using Knab.Assignment.API.Providers;
using Knab.Assignment.API.Services;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace API.Tests.Services
{
    public class CryptoServiceTests
    {
        private readonly CryptoService _sut;
        private readonly ILogger<CryptoService> _logger;
        private readonly ICryptoProvider _cryptoProvider;

        public CryptoServiceTests()
        {
            _logger = Substitute.For<ILogger<CryptoService>>();
            _cryptoProvider = Substitute.For<ICryptoProvider>();

            _sut = new CryptoService(_logger, [_cryptoProvider]);
        }

        [Fact]
        public async Task GetQuote_WhenProviderReturnsQuote_ReturnQuote()
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

            _cryptoProvider.GetQuote(cryptoCurrency, currencies).Returns(quote);

            // Act
            var result = await _sut.GetQuote(cryptoCurrency, currencies);

            // Assert
            Assert.Equal(quote, result);
        }
    }
}
