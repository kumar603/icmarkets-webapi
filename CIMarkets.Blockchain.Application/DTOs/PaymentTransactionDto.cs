/*
 * File: PaymentTransactionDto.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Data Transfer Object for payment transaction responses
 * 
 * Usage: Maps PaymentTransaction entity to API response format.
 *        Used in commands and queries results.
 *        Hides internal implementation details from API consumers.
 *        Provides consistent JSON response structure.
 * 
 * Dependencies: None (DTO with primitives)
 */

namespace CIMarkets.Blockchain.Application.DTOs
{
    /// <summary>
    /// Data Transfer Object for payment transaction responses.
    /// </summary>
    public class PaymentTransactionDto
    {
        /// <summary>
        /// Internal payment transaction ID.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// External payment gateway transaction ID.
        /// </summary>
        public string TransactionId { get; set; }

        /// <summary>
        /// Payment amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code.
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Payment status (0=Pending, 1=Processing, 2=Completed, 3=Failed, 4=Cancelled, 5=Refunded, 6=Disputed).
        /// </summary>
        public int Status { get; set; }

        /// <summary>
        /// Payment status name (Pending, Processing, Completed, etc).
        /// </summary>
        public string StatusName { get; set; }

        /// <summary>
        /// Payment method used.
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Blockchain type for crypto payments.
        /// </summary>
        public int BlockchainType { get; set; }

        /// <summary>
        /// Customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Payment description.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Error message if payment failed.
        /// </summary>
        public string? ErrorMessage { get; set; }

        /// <summary>
        /// Payment creation timestamp.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Payment completion timestamp.
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Last update timestamp.
        /// </summary>
        public DateTime? UpdatedAt { get; set; }
    }
}
