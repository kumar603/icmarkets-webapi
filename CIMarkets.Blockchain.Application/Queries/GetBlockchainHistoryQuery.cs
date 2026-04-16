/*
 * File: GetBlockchainHistoryQuery.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS query for retrieving paginated blockchain data history
 * 
 * Usage: Sent by BlockchainController to retrieve historical blockchain records.
 *        Processed by GetBlockchainHistoryQueryHandler which queries repository with pagination.
 *        Returns BlockchainHistoryResponse with records sorted by CreatedAt (descending).
 * 
 * Dependencies: BlockchainType enum, BlockchainHistoryResponse class, MediatR
 */

using CIMarkets.Blockchain.Domain.Enums;
using MediatR;

namespace CIMarkets.Blockchain.Application.Queries
{
    /// <summary>
    /// Query to retrieve blockchain history sorted by CreatedAt descending.
    /// </summary>
    public class GetBlockchainHistoryQuery : IRequest<BlockchainHistoryResponse>
    {
        public BlockchainType BlockchainType { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Response containing paginated blockchain history.
    /// </summary>
    public class BlockchainHistoryResponse
    {
        public List<BlockchainRecordDto> Records { get; set; } = new();
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    }

    /// <summary>
    /// DTO for individual blockchain record in history response.
    /// </summary>
    public class BlockchainRecordDto
    {
        public int Id { get; set; }
        public string BlockchainType { get; set; }
        public string RawJson { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
