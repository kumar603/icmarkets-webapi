using CIMarkets.Blockchain.Application.Commands.Handlers;
using CIMarkets.Blockchain.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace CIMarkets.Blockchain.Infrastructure.ExternalApis
{
    /// <summary>
    /// Configuration options for BlockCypher API integration.
    /// </summary>
    public class BlockCypherOptions
    {
        public string BaseUrl { get; set; } = "https://api.blockcypher.com/v1";
        public string ApiKey { get; set; }
        public int TimeoutSeconds { get; set; } = 30;
    }

    /// <summary>
    /// BlockCypher API client for fetching blockchain data.
    /// </summary>
    public class BlockCypherClient : IBlockCypherClient
    {
        private readonly HttpClient _httpClient;
        private readonly BlockCypherOptions _options;
        private readonly ILogger<BlockCypherClient> _logger;

        public BlockCypherClient(
            HttpClient httpClient,
            IOptions<BlockCypherOptions> options,
            ILogger<BlockCypherClient> logger)
        {
            _httpClient = httpClient;
            _options = options.Value;
            _logger = logger;
            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
        }

        /// <summary>
        /// Fetches Bitcoin data from BlockCypher API.
        /// </summary>
        public async Task<string> FetchBitcoinAsync(string? address, CancellationToken cancellationToken = default)
        {
            return await FetchBlockchainDataAsync("btc", "main", address, cancellationToken);
        }

        /// <summary>
        /// Fetches Ethereum data from BlockCypher API.
        /// </summary>
        public async Task<string> FetchEthereumAsync(string? address, CancellationToken cancellationToken = default)
        {
            return await FetchBlockchainDataAsync("eth", "main", address, cancellationToken);
        }

        /// <summary>
        /// Fetches Litecoin data from BlockCypher API.
        /// </summary>
        public async Task<string> FetchLitecoinAsync(string? address, CancellationToken cancellationToken = default)
        {
            return await FetchBlockchainDataAsync("ltc", "main", address, cancellationToken);
        }

        /// <summary>
        /// Fetches Dash data from BlockCypher API.
        /// </summary>
        public async Task<string> FetchDashAsync(string? address, CancellationToken cancellationToken = default)
        {
            return await FetchBlockchainDataAsync("dash", "main", address, cancellationToken);
        }

        /// <summary>
        /// Generic method to fetch blockchain data from BlockCypher.
        /// </summary>
        private async Task<string> FetchBlockchainDataAsync(
            string coin,
            string chain,
            string? address,
            CancellationToken cancellationToken)
        {
            try
            {
                // Construct endpoint URL
                var endpoint = string.IsNullOrEmpty(address)
                    ? $"{_options.BaseUrl}/{coin}/{chain}"
                    : $"{_options.BaseUrl}/{coin}/{chain}/addrs/{address}";

                // Add API key if available
                var url = string.IsNullOrEmpty(_options.ApiKey)
                    ? endpoint
                    : $"{endpoint}?token={_options.ApiKey}";

                _logger.LogInformation("Fetching data from BlockCypher: {Url}", endpoint);

                var response = await _httpClient.GetAsync(url, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
                    throw new ExternalApiException(
                        $"BlockCypher API returned {response.StatusCode}: {errorContent}");
                }

                var jsonData = await response.Content.ReadAsStringAsync(cancellationToken);

                _logger.LogInformation("Successfully fetched data from BlockCypher for {Coin}/{Chain}", coin, chain);

                return jsonData;
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while fetching from BlockCypher");
                throw new ExternalApiException($"Failed to connect to BlockCypher API: {ex.Message}", ex);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogError(ex, "Request to BlockCypher API was cancelled");
                throw new ExternalApiException("Request to BlockCypher API timed out", ex);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while fetching from BlockCypher");
                throw new ExternalApiException($"Unexpected error: {ex.Message}", ex);
            }
        }
    }
}
