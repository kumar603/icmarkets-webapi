using CIMarkets.Blockchain.Application.Queries;
using FluentValidation;

namespace CIMarkets.Blockchain.Application.Validators
{
    /// <summary>
    /// Validator for GetBlockchainHistoryQuery.
    /// </summary>
    public class HistoryQueryValidator : AbstractValidator<GetBlockchainHistoryQuery>
    {
        public HistoryQueryValidator()
        {
            RuleFor(x => x.BlockchainType)
                .IsInEnum()
                .WithMessage("Invalid blockchain type.");

            RuleFor(x => x.PageNumber)
                .GreaterThan(0)
                .WithMessage("Page number must be greater than 0.");

            RuleFor(x => x.PageSize)
                .GreaterThan(0)
                .LessThanOrEqualTo(100)
                .WithMessage("Page size must be between 1 and 100.");
        }
    }
}
