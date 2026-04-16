/*
 * File: IBlockCypherClient.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Interface for BlockCypher external API integration
 * 
 * Usage: Defines contract for fetching blockchain data from BlockCypher API.
 *        Implemented by BlockCypherClient in Infrastructure layer.
 *        Provides methods for each supported blockchain: Bitcoin, Ethereum, Litecoin, Dash.
 *        All methods are async to support non-blocking HTTP operations.
 * 
 * Dependencies: None (interface only)
 */

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
