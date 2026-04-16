/*
 * File: ProcessPaymentCommand.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS command to process a payment transaction
 * 
 * Usage: Mediator command for payment processing workflow.
 *        Captured and validated before being passed to handler.
 *        Contains all required parameters for payment gateway interaction.
 *        Returns PaymentTransactionDto on successful processing.
 * 
 * Dependencies: IRequest from MediatR, PaymentTransactionDto
 */

using CIMarkets.Blockchain.Application.DTOs;
using MediatR;

namespace CIMarkets.Blockchain.Application.Commands
{
    /// <summary>
    /// Command to process a payment transaction through the payment gateway.
    /// </summary>
    public class ProcessPaymentCommand : IRequest<PaymentTransactionDto>
    {
        /// <summary>
        /// The payment amount.
        /// </summary>
        public decimal Amount { get; set; }

        /// <summary>
        /// Currency code (USD, EUR, BTC, ETH, etc).
        /// </summary>
        public string Currency { get; set; }

        /// <summary>
        /// Payment method (card, wallet, crypto, bank_transfer).
        /// </summary>
        public string PaymentMethod { get; set; }

        /// <summary>
        /// Blockchain type for crypto payments (BTC=1, ETH=2, LTC=3, DASH=4).
        /// </summary>
        public int BlockchainType { get; set; }

        /// <summary>
        /// Customer identifier.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// Payment description for user reference.
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// Optional payment reference or ID from external system.
        /// </summary>
        public string? ExternalReference { get; set; }
    }
}
