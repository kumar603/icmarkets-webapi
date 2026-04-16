/*
 * File: BlockchainRecord.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Domain entity representing a blockchain data record
 * 
 * Usage: This entity is used to store blockchain data fetched from external APIs. 
 *        It contains the complete JSON response, blockchain type identifier, and creation timestamp.
 *        Used throughout the application for data persistence and retrieval.
 * 
 * Dependencies: BlockchainType enum
 */

using CIMarkets.Blockchain.Domain.Enums;

namespace CIMarkets.Blockchain.Domain.Entities
{
    /// <summary>
    /// Represents a single blockchain data record fetched from external APIs.
    /// Stores the complete JSON response with audit metadata.
    /// </summary>
    public class BlockchainRecord
    {
        /// <summary>
        /// Primary key for the record.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Type of blockchain (Bitcoin, Ethereum, Litecoin, Dash).
        /// </summary>
        public BlockchainType BlockchainType { get; set; }

        /// <summary>
        /// Raw JSON response from the external blockchain API.
        /// </summary>
        public required string RawJson { get; set; }

        /// <summary>
        /// Timestamp when the record was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
