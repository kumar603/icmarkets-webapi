namespace CIMarkets.Blockchain.Application.Commands.Handlers
{
    /// <summary>
    /// Interface for BlockCypher API client (abstract external API dependency).
    /// Implementation in Infrastructure layer: BlockCypherClient.cs
    /// </summary>
    public interface IBlockCypherClient
    {
        /// <summary>
        /// Fetches Bitcoin data from BlockCypher API.
        /// </summary>
        Task<string> FetchBitcoinAsync(string? address, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches Ethereum data from BlockCypher API.
        /// </summary>
        Task<string> FetchEthereumAsync(string? address, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches Litecoin data from BlockCypher API.
        /// </summary>
        Task<string> FetchLitecoinAsync(string? address, CancellationToken cancellationToken = default);

        /// <summary>
        /// Fetches Dash data from BlockCypher API.
        /// </summary>
        Task<string> FetchDashAsync(string? address, CancellationToken cancellationToken = default);
    }
}
