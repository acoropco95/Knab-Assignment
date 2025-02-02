using Knab.Assignment.API.Models;
using Knab.Assignment.API.Providers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NSubstitute;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace API.Tests.Providers
{
    public class ExchangeRatesProviderTests
    {
        private readonly ExchangeRatesProvider _sut;
        private readonly ILogger<ExchangeRatesProvider> _logger;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly HttpClient _httpClient;
        private readonly HttpMessageHandlerSubstitute _httpMessageHandler;
        private readonly IConfiguration _configuration;

        public ExchangeRatesProviderTests()
        {
            _logger = Substitute.For<ILogger<ExchangeRatesProvider>>();
            _httpClientFactory = Substitute.For<IHttpClientFactory>();
            _configuration = Substitute.For<IConfiguration>();

            _httpMessageHandler = Substitute.ForPartsOf<HttpMessageHandlerSubstitute>();
            _httpClient = new HttpClient(_httpMessageHandler);
            _httpClient.BaseAddress = new Uri("https://example.com");

            _httpClientFactory.CreateClient("ExchangeRates").Returns(_httpClient);

            _sut = new ExchangeRatesProvider(_logger, _httpClientFactory, _configuration);
        }

        [Fact]
        public async Task GetQuote_WhenServiceReturnsQuote_ReturnQuote()
        {
            // Arrange
            var baseCurrency = "USD";
            var currencies = new[] { "EUR", "GBP" };
            var quote = new CryptoQuote
            {
                Base = "USD",
                Rates = new Dictionary<string, decimal>
                {
                    ["EUR"] = 1.1m,
                    ["GBP"] = 0.75m
                }
            };

            var response = new HttpResponseMessage
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(quote))
            };

            _httpMessageHandler.Send(Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>()).Returns(response);

            // Act
            var result = await _sut.GetQuote(baseCurrency, currencies);

            // Assert
            Assert.Equal(quote.Base, result!.Base);
            Assert.Equal(2, result.Rates.Count);
        }
    }

    public class HttpMessageHandlerSubstitute : HttpMessageHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return Send(request, cancellationToken);
        }

        public virtual Task<HttpResponseMessage> Send(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
