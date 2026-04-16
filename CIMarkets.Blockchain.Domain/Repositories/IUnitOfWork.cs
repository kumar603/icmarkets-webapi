namespace CIMarkets.Blockchain.Domain.Repositories
{
    /// <summary>
    /// Unit of Work pattern interface for transaction management.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        IBlockchainRecordRepository BlockchainRecords { get; }

        Task CommitAsync();

        Task RollbackAsync();
    }
}
