using CIMarkets.Blockchain.Domain.Repositories;
using CIMarkets.Blockchain.Infrastructure.Data;

namespace CIMarkets.Blockchain.Infrastructure.Repositories
{
    /// <summary>
    /// Unit of Work implementation for transaction management.
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly BlockchainDbContext _context;
        private IBlockchainRecordRepository _blockchainRecordsRepository;

        public UnitOfWork(BlockchainDbContext context)
        {
            _context = context;
        }

        public IBlockchainRecordRepository BlockchainRecords
        {
            get
            {
                _blockchainRecordsRepository ??= new BlockchainRecordRepository(_context);
                return _blockchainRecordsRepository;
            }
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task RollbackAsync()
        {
            // Rollback all changes in the current transaction
            await _context.DisposeAsync();
        }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
