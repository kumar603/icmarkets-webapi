/*
 * File: PaymentTransaction.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Domain entity representing a payment transaction
 * 
 * Usage: Stores payment transaction details, status, and audit information.
 *        Used to track blockchain-related payments and transactions.
 *        Can be extended to support multiple payment gateways.
 *        Enables payment history, reconciliation, and reporting.
 * 
 * Dependencies: PaymentStatus enum
 */

using CIMarkets.Blockchain.Domain.Enums;

namespace CIMarkets.Blockchain.Domain.Entities
{
    /// <summary>
    /// Represents a payment transaction in the system.
    /// </summary>
    public class PaymentTransaction
    {
        /// <summary>
        /// Primary key for the transaction.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Unique transaction identifier from payment gateway.
        /// </summary>
        public required string TransactionId { get; set; }

        /// <summary>
        /// Amount in the transaction currency.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code (e.g., USD, EUR, BTC, ETH).
        /// </summary>
        public required string Currency { get; set; }

        /// <summary>
        /// Current status of the transaction.
        /// </summary>
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        /// <summary>
        /// Payment method used (card, wallet, crypto, etc.).
        /// </summary>
        public required string PaymentMethod { get; set; }

        /// <summary>
        /// Related blockchain type if transaction is blockchain-related.
        /// </summary>
        public int? BlockchainType { get; set; }

        /// <summary>
        /// Description or purpose of the transaction.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Customer or user identifier.
        /// </summary>
        public required string CustomerId { get; set; }

        /// <summary>
        /// Response data from payment gateway (JSON).
        /// </summary>
        public string? GatewayResponse { get; set; }

        /// <summary>
        /// Error message if transaction failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// When the transaction was created (UTC).
        /// </summary>
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the transaction was last updated (UTC).
        /// </summary>
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// When the transaction was completed (UTC).
        /// </summary>
        public DateTime? CompletedAt { get; set; }
    }
}
