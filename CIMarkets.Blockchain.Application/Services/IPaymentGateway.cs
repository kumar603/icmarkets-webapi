/*
 * File: IPaymentGateway.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Interface for payment gateway integration
 * 
 * Usage: Defines contract for payment processing operations.
 *        Implemented by PaymentGatewayClient in Infrastructure layer.
 *        Supports license-less implementation with structure for future integration.
 *        Enables polymorph payment gateway implementations.
 * 
 * Dependencies: None (interface only)
 */

namespace CIMarkets.Blockchain.Application.Services
{
    /// <summary>
    /// Interface for external payment gateway integration.
    /// </summary>
    public interface IPaymentGateway
    {
        /// <summary>
        /// Process a payment transaction.
        /// </summary>
        /// <param name="request">Payment request details</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Payment result with transaction details</returns>
        Task<PaymentResult> ProcessPaymentAsync(
            PaymentRequest request,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Refund a processed payment.
        /// </summary>
        /// <param name="transactionId">Original transaction identifier</param>
        /// <param name="amount">Amount to refund (null for full refund)</param>
        /// <param name="reason">Refund reason</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Refund result</returns>
        Task<PaymentResult> RefundPaymentAsync(
            string transactionId,
            decimal? amount = null,
            string? reason = null,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Retrieve payment transaction details.
        /// </summary>
        /// <param name="transactionId">Transaction identifier</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Transaction details</returns>
        Task<PaymentResult> GetTransactionDetailsAsync(
            string transactionId,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Validate payment method before processing.
        /// </summary>
        /// <param name="paymentMethod">Payment method to validate</param>
        /// <param name="currency">Currency code</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>True if valid, false otherwise</returns>
        Task<bool> ValidatePaymentMethod(string paymentMethod, string currency, CancellationToken cancellationToken = default);

        /// <summary>
        /// Payment request details.
        /// </summary>
        public class PaymentRequest
        {
            /// <summary>
            /// Amount to charge.
            /// </summary>
            public decimal Amount { get; set; }

            /// <summary>
            /// Currency code (USD, EUR, BTC, ETH, etc).
            /// </summary>
            public required string Currency { get; set; }

            /// <summary>
            /// Payment method (card, wallet, crypto, etc).
            /// </summary>
            public required string PaymentMethod { get; set; }

            /// <summary>
            /// Customer identifier.
            /// </summary>
            public required string CustomerId { get; set; }

            /// <summary>
            /// Transaction description.
            /// </summary>
            public string? Description { get; set; }

            /// <summary>
            /// External reference for correlation.
            /// </summary>
            public string? ExternalReference { get; set; }
        }
    }

    /// <summary>
    /// Result of a payment operation.
    /// </summary>
    public class PaymentResult
    {
        /// <summary>
        /// Whether the operation was successful.
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Transaction identifier from gateway.
        /// </summary>
        public required string TransactionId { get; set; }

        /// <summary>
        /// Transaction amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code.
        /// </summary>
        public required string Currency { get; set; }

        /// <summary>
        /// Current transaction status.
        /// </summary>
        public required string Status { get; set; }

        /// <summary>
        /// Success or error message.
        /// </summary>
        public required string Message { get; set; }

        /// <summary>
        /// Response data from payment gateway (JSON).
        /// </summary>
        public string? GatewayResponse { get; set; }

        /// <summary>
        /// Reference code for reconciliation.
        /// </summary>
        public string? ReferenceCode { get; set; }

        /// <summary>
        /// When the transaction was processed.
        /// </summary>
        public DateTime ProcessedAt { get; set; } = DateTime.UtcNow;
    }
}
