using CIMarkets.Blockchain.Domain.Repositories;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CIMarkets.Blockchain.Application.Queries.Handlers
{
    /// <summary>
    /// Handler for GetBlockchainHistoryQuery - retrieves blockchain records sorted by creation date.
    /// </summary>
    public class GetBlockchainHistoryQueryHandler : IRequestHandler<GetBlockchainHistoryQuery, BlockchainHistoryResponse>
    {
        private readonly IBlockchainRecordRepository _repository;
        private readonly ILogger<GetBlockchainHistoryQueryHandler> _logger;

        public GetBlockchainHistoryQueryHandler(
            IBlockchainRecordRepository repository,
            ILogger<GetBlockchainHistoryQueryHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<BlockchainHistoryResponse> Handle(GetBlockchainHistoryQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Fetching blockchain history for {BlockchainType}, Page {PageNumber}", 
                    request.BlockchainType, request.PageNumber);

                var (records, totalCount) = await _repository.GetHistoryAsync(
                    request.BlockchainType,
                    request.PageNumber,
                    request.PageSize);

                var response = new BlockchainHistoryResponse
                {
                    Records = records
                        .Select(r => new BlockchainRecordDto
                        {
                            Id = r.Id,
                            BlockchainType = r.BlockchainType.ToString(),
                            RawJson = r.RawJson,
                            CreatedAt = r.CreatedAt
                        })
                        .ToList(),
                    TotalCount = totalCount,
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };

                _logger.LogInformation("Retrieved {RecordCount} records out of {TotalCount} total",
                    response.Records.Count, totalCount);

                return response;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving blockchain history");
                throw;
            }
        }
    }
}
