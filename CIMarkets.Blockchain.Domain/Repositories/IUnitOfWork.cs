/*
 * File: IUnitOfWork.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Unit of Work pattern interface for transaction management
 * 
 * Usage: This interface manages repository lifecycles and transaction boundaries.
 *        Ensures atomic database operations across multiple repositories.
 *        Implemented by UnitOfWork in Infrastructure layer.
 * 
 * Dependencies: IBlockchainRecordRepository interface
 */

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
