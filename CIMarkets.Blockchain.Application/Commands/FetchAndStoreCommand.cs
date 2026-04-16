/*
 * File: FetchAndStoreCommand.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS command for fetching blockchain data from external API and storing locally
 * 
 * Usage: Sent by the BlockchainController when user requests blockchain data sync.
 *        Processed by FetchAndStoreCommandHandler which coordinates API calls and persistence.
 *        Returns CommandResult with success status, message, and stored blockchain record response.
 * 
 * Dependencies: BlockchainType enum, CommandResult class, MediatR
 */

using CIMarkets.Blockchain.Domain.Enums;
using MediatR;

namespace CIMarkets.Blockchain.Application.Commands
{
    /// <summary>
    /// Command to fetch blockchain data from external API and store it.
    /// </summary>
    public class FetchAndStoreCommand : IRequest<CommandResult>
    {
        /// <summary>
        /// Type of blockchain to fetch data for.
        /// </summary>
        public BlockchainType BlockchainType { get; set; }

        /// <summary>
        /// Optional address to fetch data for (if applicable).
        /// </summary>
        public string? Address { get; set; }
    }

    /// <summary>
    /// Result of the fetch and store operation.
    /// </summary>
    public class CommandResult
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public BlockchainRecordResponse Data { get; set; }
    }

    /// <summary>
    /// DTO response for a blockchain record.
    /// </summary>
    public class BlockchainRecordResponse
    {
        public int Id { get; set; }
        public string BlockchainType { get; set; }
        public string RawJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
