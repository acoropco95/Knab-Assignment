namespace Knab.Assignment.API.Models
{
    public class CoinMarketCapOptions
    {
        public const string Path = "AppSettings:CoinMarketCap";

        public string ApiKey { get; set; }

        public string ApiKeyHeader { get; set; }

        public string BaseUrl { get; set; }
    }
}
