/*
 * File: PaymentTransactionRepository.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Concrete repository implementation for payment transaction data access using Entity Framework Core
 * 
 * Usage: Implements IPaymentTransactionRepository with EF Core queries.
 *        Provides CRUD operations for PaymentTransaction entities.
 *        Supports filtering, aggregation, and date-range queries.
 *        Integrates with BlockchainDbContext for SQLite persistence.
 * 
 * Dependencies: BlockchainDbContext, IPaymentTransactionRepository, PaymentTransaction, PaymentStatus
 */

using CIMarkets.Blockchain.Domain.Entities;
using CIMarkets.Blockchain.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace CIMarkets.Blockchain.Infrastructure.Repositories
{
    /// <summary>
    /// Concrete repository implementation for payment transaction data access operations.
    /// </summary>
    public class PaymentTransactionRepository : IPaymentTransactionRepository
    {
        private readonly BlockchainDbContext _context;

        /// <summary>
        /// Initializes a new instance of the PaymentTransactionRepository class.
        /// </summary>
        /// <param name="context">The blockchain database context.</param>
        public PaymentTransactionRepository(BlockchainDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// Adds a new payment transaction to the repository.
        /// </summary>
        public async Task AddAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            await _context.PaymentTransactions.AddAsync(transaction, cancellationToken);
        }

        /// <summary>
        /// Updates an existing payment transaction.
        /// </summary>
        public async Task UpdateAsync(PaymentTransaction transaction, CancellationToken cancellationToken = default)
        {
            if (transaction == null)
            {
                throw new ArgumentNullException(nameof(transaction));
            }

            _context.PaymentTransactions.Update(transaction);
            await Task.CompletedTask; // Explicit async method signature
        }

        /// <summary>
        /// Retrieves a payment transaction by ID.
        /// </summary>
        public async Task<PaymentTransaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        /// <summary>
        /// Retrieves a payment transaction by transaction ID (external gateway reference).
        /// </summary>
        public async Task<PaymentTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(transactionId))
            {
                throw new ArgumentException("Transaction ID cannot be null or empty.", nameof(transactionId));
            }

            return await _context.PaymentTransactions
                .FirstOrDefaultAsync(p => p.TransactionId == transactionId, cancellationToken);
        }

        /// <summary>
        /// Retrieves all payment transactions for a specific customer.
        /// </summary>
        public async Task<IEnumerable<PaymentTransaction>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty.", nameof(customerId));
            }

            return await _context.PaymentTransactions
                .Where(p => p.CustomerId == customerId)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves payment transactions by status.
        /// </summary>
        public async Task<IEnumerable<PaymentTransaction>> GetByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .Where(p => p.Status == status)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves payment transactions within a date range.
        /// </summary>
        public async Task<IEnumerable<PaymentTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .Where(p => p.CreatedAt >= startDate && p.CreatedAt <= endDate)
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Retrieves all payment transactions.
        /// </summary>
        public async Task<IEnumerable<PaymentTransaction>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        /// <summary>
        /// Deletes a payment transaction by ID.
        /// </summary>
        public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
        {
            var transaction = await _context.PaymentTransactions
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);

            if (transaction != null)
            {
                _context.PaymentTransactions.Remove(transaction);
            }
        }

        /// <summary>
        /// Gets the count of payment transactions with a specific status.
        /// </summary>
        public async Task<int> GetCountByStatusAsync(PaymentStatus status, CancellationToken cancellationToken = default)
        {
            return await _context.PaymentTransactions
                .CountAsync(p => p.Status == status, cancellationToken);
        }

        /// <summary>
        /// Gets the total payment amount for a specific customer.
        /// </summary>
        public async Task<decimal> GetTotalAmountByCustomerAsync(string customerId, bool includeFailed = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(customerId))
            {
                throw new ArgumentException("Customer ID cannot be null or empty.", nameof(customerId));
            }

            var query = _context.PaymentTransactions
                .Where(p => p.CustomerId == customerId);

            if (!includeFailed)
            {
                query = query.Where(p => p.Status == PaymentStatus.Completed);
            }

            return await query.SumAsync(p => p.Amount, cancellationToken);
        }
    }
}
