namespace CIMarkets.Blockchain.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when external API call fails.
    /// </summary>
    public class ExternalApiException : Exception
    {
        public ExternalApiException(string message) : base(message) { }

        public ExternalApiException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
