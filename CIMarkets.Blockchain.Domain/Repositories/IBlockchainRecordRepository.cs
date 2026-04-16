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
