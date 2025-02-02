using Knab.Assignment.API.Models;

namespace Knab.Assignment.API.Providers
{
    public interface ICryptoProvider
    {
        public string Identifier { get; }

        Task<CryptoQuote?> GetQuote(string cryptoCurrency, string[] currencies);
    }
}
