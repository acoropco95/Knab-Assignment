using System.Net.Http;
using System.Text.Json;
using Knab.Assignment.API.Models;
using Microsoft.Extensions.Options;

namespace Knab.Assignment.API.Providers
{
    public class ExchangeRatesProvider : ICryptoProvider
    {
        private readonly ExchangeRatesOptions _options;
        private readonly ILogger<ExchangeRatesProvider> _logger;
        private readonly HttpClient _httpClient;

        public ExchangeRatesProvider(ILogger<ExchangeRatesProvider> logger, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _httpClient = httpClientFactory.CreateClient(Identifier);

            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Base address is not set for ExchangeRates http client!");
            }

            _logger = logger;

            _options = new ExchangeRatesOptions();
            config.GetSection(ExchangeRatesOptions.Path).Bind(_options);
        }

        public string Identifier => "ExchangeRates";

        public async Task<CryptoQuote?> GetQuote(string cryptoCurrency, string[] currencies)
        {

            var queryParams = new Dictionary<string, string>
            {
                { "access_key", _options.ApiKey },
                { "base", cryptoCurrency},
                { "symbols", string.Join(',', currencies) }
            };

            var uriBuilder = new UriBuilder(_httpClient.BaseAddress!);
            var query = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
            uriBuilder.Query = query;

            var response = await _httpClient.GetAsync(uriBuilder.Uri);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get exchange rates for {cryptoCurrency} to {string.Join(',', currencies)} using the ExchangeRates API client. Status code: {response.StatusCode}. Message: {response.ReasonPhrase}");
                return null;
            }

            var json = response.Content.ReadAsStringAsync().Result;
            var quote = JsonSerializer.Deserialize<CryptoQuote>(json);

            if (quote != null)
                quote.Source = Identifier;

            return quote;
        }
    }
}
