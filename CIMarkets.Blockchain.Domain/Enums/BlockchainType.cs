/*
 * File: BlockchainType.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Domain enum for supported blockchain types
 * 
 * Usage: This enumeration defines all supported blockchain networks.
 *        Used in BlockchainRecord entity and throughout the application for type discrimination.
 *        Values: Bitcoin (1), Ethereum (2), Litecoin (3), Dash (4)
 * 
 * Dependencies: None (pure enum)
 */

namespace CIMarkets.Blockchain.Domain.Enums
{
    /// <summary>
    /// Represents the blockchain type for data fetching and storage.
    /// </summary>
    public enum BlockchainType
    {
        Bitcoin = 1,
        Ethereum = 2,
        Litecoin = 3,
        Dash = 4
    }
}
