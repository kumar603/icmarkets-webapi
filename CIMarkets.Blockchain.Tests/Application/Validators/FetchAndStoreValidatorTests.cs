using CIMarkets.Blockchain.Application.Commands;
using CIMarkets.Blockchain.Application.Validators;
using CIMarkets.Blockchain.Domain.Enums;
using FluentValidation;
using Xunit;

namespace CIMarkets.Blockchain.Tests.Application.Validators
{
    /// <summary>
    /// Unit tests for FetchAndStoreValidator.
    /// </summary>
    public class FetchAndStoreValidatorTests
    {
        private readonly FetchAndStoreValidator _validator;

        public FetchAndStoreValidatorTests()
        {
            _validator = new FetchAndStoreValidator();
        }

        [Fact]
        public async Task Validate_WithValidBitcoinCommand_ShouldPass()
        {
            // Arrange
            var command = new FetchAndStoreCommand
            {
                BlockchainType = BlockchainType.Bitcoin,
                Address = "3J98t1WpEZ73CNmYviecrnyiWrnqRhWNLy"
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            Assert.True(result.IsValid);
        }

        [Fact]
        public async Task Validate_WithInvalidBlockchainType_ShouldFail()
        {
            // Arrange
            var command = new FetchAndStoreCommand
            {
                BlockchainType = (BlockchainType)99, // Invalid enum value
                Address = "test"
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
            Assert.NotEmpty(result.Errors);
        }

        [Fact]
        public async Task Validate_WithAddressExceedingMaxLength_ShouldFail()
        {
            // Arrange
            var command = new FetchAndStoreCommand
            {
                BlockchainType = BlockchainType.Ethereum,
                Address = new string('a', 201) // Exceeds 200 character limit
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            Assert.False(result.IsValid);
        }

        [Fact]
        public async Task Validate_WithNullAddress_ShouldPass()
        {
            // Arrange
            var command = new FetchAndStoreCommand
            {
                BlockchainType = BlockchainType.Litecoin,
                Address = null
            };

            // Act
            var result = await _validator.ValidateAsync(command);

            // Assert
            Assert.True(result.IsValid);
        }
    }
}
