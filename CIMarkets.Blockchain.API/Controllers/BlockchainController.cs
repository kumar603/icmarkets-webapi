using CIMarkets.Blockchain.Application.Commands;
using CIMarkets.Blockchain.Application.Commands.Handlers;
using CIMarkets.Blockchain.Application.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace CIMarkets.Blockchain.API.Controllers
{
    /// <summary>
    /// API controller for blockchain data operations.
    /// Handles fetching data from external APIs and retrieving history.
    /// </summary>
    [ApiController]
    [Route("api/v1/[controller]")]
    public class BlockchainController : ControllerBase
    {
        private readonly IMediator _mediator;
        private readonly ILogger<BlockchainController> _logger;

        public BlockchainController(IMediator mediator, ILogger<BlockchainController> logger)
        {
            _mediator = mediator;
            _logger = logger;
        }

        /// <summary>
        /// Fetches blockchain data from external API and stores it in the database.
        /// </summary>
        /// <param name="request">Request containing blockchain type and optional address</param>
        /// <returns>Result containing the stored record details</returns>
        [HttpPost("sync")]
        [ProducesResponseType(typeof(CommandResult), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<CommandResult>> FetchAndStoreAsync(
            [FromBody] FetchBlockchainDataRequest request)
        {
            _logger.LogInformation("Received fetch and store request for {BlockchainType}", request.BlockchainType);

            var command = new FetchAndStoreCommand
            {
                BlockchainType = request.BlockchainType,
                Address = request.Address
            };

            var result = await _mediator.Send(command);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Retrieves blockchain history sorted by creation date (most recent first).
        /// </summary>
        /// <param name="blockchainType">Type of blockchain (Bitcoin, Ethereum, Litecoin, Dash)</param>
        /// <param name="pageNumber">Page number for pagination (default: 1)</param>
        /// <param name="pageSize">Number of records per page (default: 50, max: 100)</param>
        /// <returns>Paginated blockchain history</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(BlockchainHistoryResponse), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<BlockchainHistoryResponse>> GetHistoryAsync(
            [FromQuery] int blockchainType,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 50)
        {
            _logger.LogInformation("Retrieving blockchain history for type {BlockchainType}, page {PageNumber}",
                blockchainType, pageNumber);

            var query = new GetBlockchainHistoryQuery
            {
                BlockchainType = (Domain.Enums.BlockchainType)blockchainType,
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }

    /// <summary>
    /// Request DTO for fetching blockchain data.
    /// </summary>
    public class FetchBlockchainDataRequest
    {
        public Domain.Enums.BlockchainType BlockchainType { get; set; }
        public string? Address { get; set; }
    }
}
