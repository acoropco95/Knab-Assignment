using Knab.Assignment.API.Models;
using System.Net.Http;
using System.Text.Json;

namespace Knab.Assignment.API.Providers
{
    public class CoinMarketCapProvider : ICryptoProvider
    {
        private readonly ILogger<CoinMarketCapProvider> _logger;
        private readonly HttpClient _httpClient;

        public CoinMarketCapProvider(ILogger<CoinMarketCapProvider> logger, IHttpClientFactory httpClientFactory)
        {
            _httpClient = httpClientFactory.CreateClient(Identifier);

            if (_httpClient.BaseAddress == null)
            {
                throw new Exception("Base address is not set for CoinMarketCap http client!");
            }

            _logger = logger;
        }

        public string Identifier => "CoinMarketCap";

        public async Task<CryptoQuote?> GetQuote(string cryptoCurrency, string[] currencies)
        {
            var queryParams = new Dictionary<string, string>
            {
                { "symbol", cryptoCurrency },
                { "convert", string.Join(',', currencies) }
            };

            var uriBuilder = new UriBuilder(_httpClient.BaseAddress!);
            var query = new FormUrlEncodedContent(queryParams).ReadAsStringAsync().Result;
            uriBuilder.Query = query;

            var response = await _httpClient.GetAsync(uriBuilder.Uri);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError($"Failed to get exchange rates for {cryptoCurrency} to {string.Join(',', currencies)} using the CoinMarketCap API client. Status code: {response.StatusCode}. Message: {response.ReasonPhrase}");
                return null;
            }

            var json = response.Content.ReadAsStringAsync().Result;

            using JsonDocument document = JsonDocument.Parse(json);
            var cryptoQuote = new CryptoQuote
            {
                Source = Identifier,
                Base = cryptoCurrency.ToUpper(),
                Rates = []
            };

            var quote = document.RootElement.GetProperty("data").GetProperty(cryptoCurrency.ToUpper()).GetProperty("quote");

            for (int i = 0; i < currencies.Length; i++)
            {
                var currency = quote.GetProperty(currencies[i].ToUpper());
                var price = currency.GetProperty("price").GetDecimal();

                cryptoQuote.Rates.Add(currencies[i].ToUpper(), price);
            }

            return cryptoQuote;
        }
    }
}
