using Knab.Assignment.API.Models;
using Knab.Assignment.API.Providers;

namespace Knab.Assignment.API.Services
{
    public interface ICryptoService
    {
        Task<CryptoQuote?> GetQuote(string symbol, string[] currencies);
    }

    public class CryptoService : ICryptoService
    {
        private readonly ILogger<CryptoService> _logger;
        private readonly IEnumerable<ICryptoProvider> _cryptoProviders;

        public CryptoService(ILogger<CryptoService> logger, IEnumerable<ICryptoProvider> cryptoProviders)
        {
            _logger = logger;
            _cryptoProviders = cryptoProviders;
        }

        public async Task<CryptoQuote?> GetQuote(string cryptoCurrency, string[] currencies)
        {
            foreach (var provider in _cryptoProviders)
            {
                try
                {
                    var quote = await provider.GetQuote(cryptoCurrency, currencies);
                    if (quote == null)
                    {
                        _logger.LogError($"Failed to get quote for {cryptoCurrency} to {string.Join(", ", currencies)} using {provider.GetType().Name} provider.");
                        continue;
                    }

                    return quote;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, $"Exception was raised when trying to get quote for {cryptoCurrency} to {string.Join(", ", currencies)} using {provider.GetType().Name} provider.");
                }
            }

            return null;
        }
    }
}
