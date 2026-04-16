/*
 * File: ProcessPaymentCommandValidator.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: FluentValidation validator for ProcessPaymentCommand
 * 
 * Usage: Validates payment command parameters before processing.
 *        Ensures amount is positive, required fields are populated.
 *        Runs before handler execution via MediatR pipeline.
 *        Provides meaningful validation error messages.
 * 
 * Dependencies: AbstractValidator from FluentValidation, ProcessPaymentCommand
 */

using FluentValidation;

namespace CIMarkets.Blockchain.Application.Commands.Validators
{
    /// <summary>
    /// Validator for ProcessPaymentCommand.
    /// </summary>
    public class ProcessPaymentCommandValidator : AbstractValidator<ProcessPaymentCommand>
    {
        /// <summary>
        /// Initializes validation rules for ProcessPaymentCommand.
        /// </summary>
        public ProcessPaymentCommandValidator()
        {
            RuleFor(x => x.Amount)
                .GreaterThan(0)
                .WithMessage("Payment amount must be greater than 0");

            RuleFor(x => x.Currency)
                .NotEmpty()
                .WithMessage("Currency is required")
                .Length(2, 10)
                .WithMessage("Currency code must be 2-10 characters");

            RuleFor(x => x.PaymentMethod)
                .NotEmpty()
                .WithMessage("Payment method is required")
                .Must(x => x == "card" || x == "wallet" || x == "crypto" || x == "bank_transfer")
                .WithMessage("Payment method must be card, wallet, crypto, or bank_transfer");

            RuleFor(x => x.CustomerId)
                .NotEmpty()
                .WithMessage("Customer ID is required")
                .MaximumLength(100)
                .WithMessage("Customer ID must not exceed 100 characters");

            RuleFor(x => x.BlockchainType)
                .GreaterThan(0)
                .WithMessage("Valid blockchain type is required");

            RuleFor(x => x.Description)
                .MaximumLength(500)
                .WithMessage("Description must not exceed 500 characters")
                .When(x => !string.IsNullOrEmpty(x.Description));

            RuleFor(x => x.ExternalReference)
                .MaximumLength(100)
                .WithMessage("External reference must not exceed 100 characters")
                .When(x => !string.IsNullOrEmpty(x.ExternalReference));
        }
    }
}
