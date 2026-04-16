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
