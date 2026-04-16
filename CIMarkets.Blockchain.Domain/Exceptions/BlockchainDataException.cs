namespace CIMarkets.Blockchain.Domain.Exceptions
{
    /// <summary>
    /// Exception thrown when blockchain data operation fails.
    /// </summary>
    public class BlockchainDataException : Exception
    {
        public BlockchainDataException(string message) : base(message) { }

        public BlockchainDataException(string message, Exception innerException)
            : base(message, innerException) { }
    }
}
