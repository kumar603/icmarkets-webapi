/*
 * File: GetPaymentHistoryQuery.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS query to retrieve payment transaction history
 * 
 * Usage: Mediator query for fetching historical payment records.
 *        Filtered by customer ID and optional date range.
 *        Returns collection of PaymentTransactionDto objects.
 *        Supports pagination via Skip and Take parameters.
 * 
 * Dependencies: IRequest from MediatR, PaymentTransactionDto
 */

using CIMarkets.Blockchain.Application.DTOs;
using MediatR;

namespace CIMarkets.Blockchain.Application.Queries
{
    /// <summary>
    /// Query to retrieve payment transaction history for a customer.
    /// </summary>
    public class GetPaymentHistoryQuery : IRequest<IEnumerable<PaymentTransactionDto>>
    {
        /// <summary>
        /// Customer identifier to filter payment history.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Optional start date for filtering (inclusive).
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// Optional end date for filtering (inclusive).
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// Optional payment status filter (null = all statuses).
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// Number of records to skip for pagination.
        /// </summary>
        public int Skip { get; set; } = 0;

        /// <summary>
        /// Number of records to take for pagination.
        /// </summary>
        public int Take { get; set; } = 50;
    }
}
