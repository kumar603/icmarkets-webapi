/*
 * File: IPaymentTransactionRepository.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Repository interface for payment transaction data access operations
 * 
 * Usage: Defines contract for payment transaction persistence.
 *        Implements repository pattern with async operations.
 *        Supports CRUD operations and query filtering.
 *        Integrates with Entity Framework Core for SQLite storage.
 * 
 * Dependencies: PaymentTransaction (domain entity)
 */

namespace CIMarkets.Blockchain.Infrastructure.Repositories
{
    /// <summary>
    /// Repository interface for payment transaction data access operations.
    /// </summary>
    public interface IPaymentTransactionRepository
    {
        /// <summary>
        /// Adds a new payment transaction to the repository.
        /// </summary>
        /// <param name="transaction">The payment transaction to add.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task AddAsync(Domain.Entities.PaymentTransaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Updates an existing payment transaction.
        /// </summary>
        /// <param name="transaction">The payment transaction to update.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task UpdateAsync(Domain.Entities.PaymentTransaction transaction, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a payment transaction by ID.
        /// </summary>
        /// <param name="id">The payment transaction ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Payment transaction if found; otherwise null.</returns>
        Task<Domain.Entities.PaymentTransaction?> GetByIdAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves a payment transaction by transaction ID (external gateway reference).
        /// </summary>
        /// <param name="transactionId">The external transaction ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Payment transaction if found; otherwise null.</returns>
        Task<Domain.Entities.PaymentTransaction?> GetByTransactionIdAsync(string transactionId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all payment transactions for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of payment transactions for the customer.</returns>
        Task<IEnumerable<Domain.Entities.PaymentTransaction>> GetByCustomerIdAsync(string customerId, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves payment transactions by status.
        /// </summary>
        /// <param name="status">The payment status to filter by.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of payment transactions with the specified status.</returns>
        Task<IEnumerable<Domain.Entities.PaymentTransaction>> GetByStatusAsync(Domain.Enums.PaymentStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves payment transactions within a date range.
        /// </summary>
        /// <param name="startDate">The start date (inclusive).</param>
        /// <param name="endDate">The end date (inclusive).</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of payment transactions within the date range.</returns>
        Task<IEnumerable<Domain.Entities.PaymentTransaction>> GetByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieves all payment transactions.
        /// </summary>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Collection of all payment transactions.</returns>
        Task<IEnumerable<Domain.Entities.PaymentTransaction>> GetAllAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Deletes a payment transaction by ID.
        /// </summary>
        /// <param name="id">The payment transaction ID.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Task representing the asynchronous operation.</returns>
        Task DeleteAsync(int id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the count of payment transactions with a specific status.
        /// </summary>
        /// <param name="status">The payment status to count.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Count of payment transactions with the specified status.</returns>
        Task<int> GetCountByStatusAsync(Domain.Enums.PaymentStatus status, CancellationToken cancellationToken = default);

        /// <summary>
        /// Gets the total payment amount for a specific customer.
        /// </summary>
        /// <param name="customerId">The customer ID.</param>
        /// <param name="includeFailed">Whether to include failed transactions in the total.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>Total payment amount for the customer.</returns>
        Task<decimal> GetTotalAmountByCustomerAsync(string customerId, bool includeFailed = false, CancellationToken cancellationToken = default);
    }
}
