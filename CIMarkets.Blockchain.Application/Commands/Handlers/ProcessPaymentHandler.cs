/*
 * File: ProcessPaymentHandler.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS command handler for processing payment transactions
 * 
 * Usage: Handles ProcessPaymentCommand to orchestrate payment processing.
 *        Interacts with payment gateway and repository.
 *        Creates PaymentTransaction entity and stores in database.
 *        Logs all payment operations for audit trail.
 *        Returns PaymentTransactionDto response to caller.
 * 
 * Dependencies: IPaymentGateway, IPaymentTransactionRepository, IUnitOfWork,
 *              IMapper, ILogger, PaymentTransaction, PaymentStatus
 */

using AutoMapper;
using CIMarkets.Blockchain.Application.DTOs;
using CIMarkets.Blockchain.Application.Services;
using CIMarkets.Blockchain.Domain.Entities;
using CIMarkets.Blockchain.Domain.Enums;
using CIMarkets.Blockchain.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CIMarkets.Blockchain.Application.Commands.Handlers
{
    /// <summary>
    /// Handler for processing payment transactions through payment gateway.
    /// </summary>
    public class ProcessPaymentHandler : IRequestHandler<ProcessPaymentCommand, PaymentTransactionDto>
    {
        private readonly IPaymentGateway _paymentGateway;
        private readonly IPaymentTransactionRepository _paymentRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<ProcessPaymentHandler> _logger;

        /// <summary>
        /// Initializes a new instance of ProcessPaymentHandler.
        /// </summary>
        public ProcessPaymentHandler(
            IPaymentGateway paymentGateway,
            IPaymentTransactionRepository paymentRepository,
            IUnitOfWork unitOfWork,
            IMapper mapper,
            ILogger<ProcessPaymentHandler> logger)
        {
            _paymentGateway = paymentGateway ?? throw new ArgumentNullException(nameof(paymentGateway));
            _paymentRepository = paymentRepository ?? throw new ArgumentNullException(nameof(paymentRepository));
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Handles the payment processing command.
        /// </summary>
        public async Task<PaymentTransactionDto> Handle(
            ProcessPaymentCommand command,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Processing payment: Amount={Amount}, Currency={Currency}, Customer={CustomerId}, Method={PaymentMethod}",
                    command.Amount, command.Currency, command.CustomerId, command.PaymentMethod);

                // Validate payment method
                var isValidMethod = await _paymentGateway.ValidatePaymentMethod(
                    command.PaymentMethod,
                    command.Currency,
                    cancellationToken);

                if (!isValidMethod)
                {
                    _logger.LogWarning(
                        "Invalid payment method: {PaymentMethod} for currency {Currency}",
                        command.PaymentMethod, command.Currency);

                    var failedTransaction = new PaymentTransaction
                    {
                        TransactionId = Guid.NewGuid().ToString(),
                        Amount = command.Amount,
                        Currency = command.Currency,
                        Status = PaymentStatus.Failed,
                        PaymentMethod = command.PaymentMethod,
                        BlockchainType = (BlockchainType)command.BlockchainType,
                        CustomerId = command.CustomerId,
                        Description = command.Description,
                        ErrorMessage = "Invalid payment method for the specified currency",
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow
                    };

                    await _paymentRepository.AddAsync(failedTransaction, cancellationToken);
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    return MapToDto(failedTransaction);
                }

                // Process payment through gateway
                var paymentResult = await _paymentGateway.ProcessPaymentAsync(
                    new IPaymentGateway.PaymentRequest
                    {
                        Amount = command.Amount,
                        Currency = command.Currency,
                        PaymentMethod = command.PaymentMethod,
                        CustomerId = command.CustomerId,
                        Description = command.Description,
                        ExternalReference = command.ExternalReference
                    },
                    cancellationToken);

                // Create payment transaction entity
                var paymentStatus = paymentResult.IsSuccess ? PaymentStatus.Completed : PaymentStatus.Failed;

                var transaction = new PaymentTransaction
                {
                    TransactionId = paymentResult.TransactionId,
                    Amount = paymentResult.Amount,
                    Currency = paymentResult.Currency,
                    Status = paymentStatus,
                    PaymentMethod = command.PaymentMethod,
                    BlockchainType = (BlockchainType)command.BlockchainType,
                    CustomerId = command.CustomerId,
                    Description = command.Description,
                    GatewayResponse = paymentResult.GatewayResponse,
                    ErrorMessage = paymentResult.IsSuccess ? null : paymentResult.Message,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    CompletedAt = paymentResult.IsSuccess ? DateTime.UtcNow : null
                };

                // Store transaction in database
                await _paymentRepository.AddAsync(transaction, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                _logger.LogInformation(
                    "Payment processed successfully: TransactionId={TransactionId}, Status={Status}, Amount={Amount}",
                    transaction.TransactionId, transaction.Status, transaction.Amount);

                return MapToDto(transaction);
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Error processing payment for customer {CustomerId}: {ErrorMessage}",
                    command.CustomerId, ex.Message);

                throw;
            }
        }

        /// <summary>
        /// Maps PaymentTransaction entity to PaymentTransactionDto.
        /// </summary>
        private PaymentTransactionDto MapToDto(PaymentTransaction transaction)
        {
            var dto = _mapper.Map<PaymentTransactionDto>(transaction);
            dto.StatusName = GetStatusName(transaction.Status);
            return dto;
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
