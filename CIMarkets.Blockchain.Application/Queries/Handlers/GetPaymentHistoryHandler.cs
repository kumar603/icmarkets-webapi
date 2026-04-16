/*
 * File: GetPaymentHistoryHandler.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS query handler for retrieving payment transaction history
 * 
 * Usage: Handles GetPaymentHistoryQuery to fetch payment records.
 *        Filters by customer ID, optional date range, and status.
 *        Supports pagination with Skip and Take.
 *        Maps PaymentTransaction entities to PaymentTransactionDto responses.
 *        Logs query execution for audit trail.
 * 
 * Dependencies: IPaymentTransactionRepository, IMapper, ILogger,
 *              GetPaymentHistoryQuery, PaymentTransactionDto
 */

using AutoMapper;
using CIMarkets.Blockchain.Application.DTOs;
using CIMarkets.Blockchain.Domain.Enums;
using CIMarkets.Blockchain.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CIMarkets.Blockchain.Application.Queries.Handlers
{
    /// <summary>
    /// Handler for retrieving payment transaction history.
    /// </summary>
    public class GetPaymentHistoryHandler : IRequestHandler<GetPaymentHistoryQuery, IEnumerable<PaymentTransactionDto>>
    {
        private readonly IPaymentTransactionRepository _paymentRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<GetPaymentHistoryHandler> _logger;

        /// <summary>
        /// Initializes a new instance of GetPaymentHistoryHandler.
        /// </summary>
        public GetPaymentHistoryHandler(
            IPaymentTransactionRepository paymentRepository,
            IMapper mapper,
            ILogger<GetPaymentHistoryHandler> logger)
        {
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the get payment history query.
        /// </summary>
        public async Task<IEnumerable<PaymentTransactionDto>> Handle(
            GetPaymentHistoryQuery query,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Retrieving payment history for customer {CustomerId}: StartDate={StartDate}, EndDate={EndDate}, Status={Status}",
                    query.CustomerId, query.StartDate, query.EndDate, query.Status);

                IEnumerable<CIMarkets.Blockchain.Domain.Entities.PaymentTransaction> transactions;

                // Apply filters
                if (query.Status.HasValue)
                {
                    var status = (PaymentStatus)query.Status.Value;
                    var allTransactions = await _paymentRepository.GetByStatusAsync(status, cancellationToken);
                    transactions = allTransactions.Where(t => t.CustomerId == query.CustomerId);
                }
                else if (query.StartDate.HasValue && query.EndDate.HasValue)
                {
                    var dateRangeTransactions = await _paymentRepository.GetByDateRangeAsync(
                        query.StartDate.Value,
                        query.EndDate.Value,
                        cancellationToken);
                    transactions = dateRangeTransactions.Where(t => t.CustomerId == query.CustomerId);
                }
                else
                {
                    transactions = await _paymentRepository.GetByCustomerIdAsync(query.CustomerId, cancellationToken);
                }

                // Apply pagination
                var paginatedTransactions = transactions
                    .Skip(query.Skip)
                    .Take(query.Take)
                    .ToList();

                // Map to DTOs
                var dtos = paginatedTransactions.Select(t =>
                {
                    var dto = _mapper.Map<PaymentTransactionDto>(t);
                    dto.StatusName = GetStatusName(t.Status);
                    return dto;
                }).ToList();

                _logger.LogInformation(
                    "Retrieved {Count} payment records for customer {CustomerId}",
                    dtos.Count, query.CustomerId);

                return dtos;
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error retrieving payment history for customer {CustomerId}",
                    query.CustomerId);

                throw;
            }
        }

        /// <summary>
        /// Gets the display name for payment status.
        /// </summary>
        private static string GetStatusName(PaymentStatus status) => status switch
        {
            PaymentStatus.Pending => "Pending",
            PaymentStatus.Processing => "Processing",
            PaymentStatus.Completed => "Completed",
            PaymentStatus.Failed => "Failed",
            PaymentStatus.Cancelled => "Cancelled",
            PaymentStatus.Refunded => "Refunded",
            PaymentStatus.Disputed => "Disputed",
            _ => "Unknown"
        };
    }
}
