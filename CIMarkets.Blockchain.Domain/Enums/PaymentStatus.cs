/*
 * File: PaymentStatus.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Enumeration for payment transaction statuses
 * 
 * Usage: Defines all possible payment transaction states.
 *        Used throughout the application for payment workflow logic.
 *        Enables proper handling of transaction lifecycle.
 * 
 * Dependencies: None (pure enum)
 */

namespace CIMarkets.Blockchain.Domain.Enums
{
    /// <summary>
    /// Represents the status of a payment transaction.
    /// </summary>
    public enum PaymentStatus
    {
        /// <summary>
        /// Transaction initiated but not yet processed.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Transaction is being processed by payment gateway.
        /// </summary>
        Processing = 1,

        /// <summary>
        /// Transaction completed successfully.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Transaction failed during processing.
        /// </summary>
        Failed = 3,

        /// <summary>
        /// Transaction was cancelled by user or system.
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Transaction was refunded.
        /// </summary>
        Refunded = 5,

        /// <summary>
        /// Transaction is under review or dispute.
        /// </summary>
        Disputed = 6
    }
}
