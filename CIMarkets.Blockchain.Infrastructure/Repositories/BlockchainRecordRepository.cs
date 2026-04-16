using CIMarkets.Blockchain.Domain.Entities;
using CIMarkets.Blockchain.Domain.Enums;
using CIMarkets.Blockchain.Domain.Repositories;
using CIMarkets.Blockchain.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace CIMarkets.Blockchain.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for BlockchainRecord data access.
    /// </summary>
    public class BlockchainRecordRepository : IBlockchainRecordRepository
    {
        private readonly BlockchainDbContext _context;

        public BlockchainRecordRepository(BlockchainDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves a blockchain record by its ID.
        /// </summary>
        public async Task<BlockchainRecord> GetByIdAsync(int id)
        {
            return await _context.BlockchainRecords
                .FirstOrDefaultAsync(r => r.Id == id);
        }

        /// <summary>
        /// Retrieves paginated blockchain history sorted by CreatedAt descending.
        /// </summary>
        public async Task<(IEnumerable<BlockchainRecord> Records, int TotalCount)> GetHistoryAsync(
            BlockchainType blockchainType,
            int pageNumber,
            int pageSize)
        {
            var query = _context.BlockchainRecords
                .Where(r => r.BlockchainType == blockchainType)
                .OrderByDescending(r => r.CreatedAt);

            var totalCount = await query.CountAsync();

            var records = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (records, totalCount);
        }

        /// <summary>
        /// Adds a new blockchain record to the database.
        /// </summary>
        public async Task AddAsync(BlockchainRecord record)
        {
            await _context.BlockchainRecords.AddAsync(record);
        }

        /// <summary>
        /// Saves all changes to the database.
        /// </summary>
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
