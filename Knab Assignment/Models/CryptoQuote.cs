namespace Knab.Assignment.API.Models
{
    public class CryptoQuote
    {
        public string Source { get; set; }

        public string Base { get; set; }

        public Dictionary<string, decimal> Rates { get; set; }
    }
}
