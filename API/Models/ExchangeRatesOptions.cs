namespace Knab.Assignment.API.Models
{
    public class ExchangeRatesOptions
    {
        public const string Path = "AppSettings:ExchangeRates";

        public string ApiKey { get; set; }

        public string BaseUrl { get; set; }
    }
}
