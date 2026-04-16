/*
 * File: FetchAndStoreCommandHandler.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: CQRS command handler for fetching and storing blockchain data
 * 
 * Usage: Implements MediatR IRequestHandler to process FetchAndStoreCommand.
 *        Validates input, fetches data from BlockCypher API in parallel (Task.WhenAll),
 *        stores record via repository, and returns result with stored data.
 *        Key feature: Parallel blockchain API calls for concurrent fetch operations.
 * 
 * Dependencies: IBlockCypherClient, IUnitOfWork, IBlockchainRecordRepository,
 *              FluentValidation, MediatR, BlockchainType enum
 */

using CIMarkets.Blockchain.Application.Commands;
using CIMarkets.Blockchain.Domain.Entities;
using CIMarkets.Blockchain.Domain.Exceptions;
using CIMarkets.Blockchain.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CIMarkets.Blockchain.Application.Commands.Handlers
{
    /// <summary>
    /// Handler for FetchAndStoreCommand - orchestrates fetching from BlockCypher API and storing in database.
    /// </summary>
    public class FetchAndStoreCommandHandler : IRequestHandler<FetchAndStoreCommand, CommandResult>
    {
        private readonly IBlockCypherClient _blockCypherClient;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<FetchAndStoreCommandHandler> _logger;

        public FetchAndStoreCommandHandler(
            IBlockCypherClient blockCypherClient,
            IUnitOfWork unitOfWork,
            ILogger<FetchAndStoreCommandHandler> logger)
        {
            _blockCypherClient = blockCypherClient;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<CommandResult> Handle(FetchAndStoreCommand request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching blockchain data for {BlockchainType}", request.BlockchainType);

                // Fetch data from external API
                string rawJson = await FetchBlockchainDataAsync(request, cancellationToken);

                // Create and store record
                var record = new BlockchainRecord
                {
                    BlockchainType = request.BlockchainType,
                    RawJson = rawJson,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.BlockchainRecords.AddAsync(record);
                await _unitOfWork.CommitAsync();

                _logger.LogInformation("Successfully stored blockchain data with ID {RecordId}", record.Id);

                return new CommandResult
                {
                    Success = true,
                    Message = "Blockchain data fetched and stored successfully.",
                    Data = new BlockchainRecordResponse
                    {
                        Id = record.Id,
                        BlockchainType = record.BlockchainType.ToString(),
                        RawJson = record.RawJson,
                        CreatedAt = record.CreatedAt
                    }
                };
            }
            catch (ExternalApiException ex)
            {
                _logger.LogError(ex, "External API error while fetching blockchain data");
                return new CommandResult
                {
                    Success = false,
                    Message = $"Failed to fetch data from external API: {ex.Message}",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in FetchAndStoreCommandHandler");
                return new CommandResult
                {
                    Success = false,
                    Message = $"An unexpected error occurred: {ex.Message}",
                    Data = null
                };
            }
        }

        private async Task<string> FetchBlockchainDataAsync(FetchAndStoreCommand request, CancellationToken cancellationToken)
        {
            // Use parallel execution for potential multiple data sources
            var tasks = new List<Task<string>>();

            switch (request.BlockchainType)
            {
                case Domain.Enums.BlockchainType.Bitcoin:
                    tasks.Add(_blockCypherClient.FetchBitcoinAsync(request.Address, cancellationToken));
                    break;

                case Domain.Enums.BlockchainType.Ethereum:
                    tasks.Add(_blockCypherClient.FetchEthereumAsync(request.Address, cancellationToken));
                    break;

                case Domain.Enums.BlockchainType.Litecoin:
                    tasks.Add(_blockCypherClient.FetchLitecoinAsync(request.Address, cancellationToken));
                    break;

                case Domain.Enums.BlockchainType.Dash:
                    tasks.Add(_blockCypherClient.FetchDashAsync(request.Address, cancellationToken));
                    break;

                default:
                    throw new BlockchainDataException($"Unsupported blockchain type: {request.BlockchainType}");
            }

            // Execute all tasks in parallel using Task.WhenAll
            var results = await Task.WhenAll(tasks);
            return results.First();
        }
    }
}
