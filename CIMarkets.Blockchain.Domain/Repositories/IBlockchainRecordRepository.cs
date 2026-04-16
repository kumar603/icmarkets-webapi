/*
 * File: IBlockchainRecordRepository.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Repository interface for blockchain record data access abstraction
 * 
 * Usage: This interface defines the contract for data access operations on blockchain records.
 *        Implemented by BlockchainRecordRepository in Infrastructure layer.
 *        Provides methods for CRUD operations and history queries with pagination.
 * 
 * Dependencies: BlockchainRecord entity, BlockchainType enum
 */

using CIMarkets.Blockchain.Domain.Entities;
using CIMarkets.Blockchain.Domain.Enums;

namespace CIMarkets.Blockchain.Domain.Repositories
{
    /// <summary>
    /// Repository interface for BlockchainRecord data access operations.
    /// </summary>
    public interface IBlockchainRecordRepository
    {
        Task<BlockchainRecord> GetByIdAsync(int id);

        Task<(IEnumerable<BlockchainRecord> Records, int TotalCount)> GetHistoryAsync(
            BlockchainType blockchainType, 
            int pageNumber, 
            int pageSize);

        Task AddAsync(BlockchainRecord record);

        Task SaveChangesAsync();
    }
}
