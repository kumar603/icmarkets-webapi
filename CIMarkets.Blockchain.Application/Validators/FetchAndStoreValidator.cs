/*
 * File: FetchAndStoreValidator.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: FluentValidation validator for FetchAndStoreCommand
 * 
 * Usage: Validates user input before processing FetchAndStoreCommand.
 *        Ensures blockchain type is valid and address format is correct.
 *        Prevents invalid requests from reaching the command handler.
 * 
 * Dependencies: FluentValidation, FetchAndStoreCommand, BlockchainType enum
 */

using CIMarkets.Blockchain.Application.Commands;
using CIMarkets.Blockchain.Domain.Enums;
using FluentValidation;

namespace CIMarkets.Blockchain.Application.Validators
{
    /// <summary>
    /// Validator for FetchAndStoreCommand.
    /// </summary>
    public class FetchAndStoreValidator : AbstractValidator<FetchAndStoreCommand>
    {
        public FetchAndStoreValidator()
        {
            RuleFor(x => x.BlockchainType)
                .IsInEnum()
                .WithMessage("Invalid blockchain type. Must be Bitcoin, Ethereum, Litecoin, or Dash.");

            RuleFor(x => x.Address)
                .MaximumLength(200)
                .When(x => !string.IsNullOrEmpty(x.Address))
                .WithMessage("Address cannot exceed 200 characters.");
        }
    }
}
