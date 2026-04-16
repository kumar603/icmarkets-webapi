using CIMarkets.Blockchain.Application.Commands.Handlers;
using CIMarkets.Blockchain.Domain.Repositories;
using CIMarkets.Blockchain.Infrastructure.Data;
using CIMarkets.Blockchain.Infrastructure.ExternalApis;
using CIMarkets.Blockchain.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CIMarkets.Blockchain.Infrastructure.Configuration
{
    /// <summary>
    /// Dependency Injection setup for Infrastructure layer.
    /// </summary>
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            // Database
            var connectionString = configuration.GetConnectionString("DefaultConnection") 
                ?? "Data Source=blockchain.db";
            
            services.AddDbContext<BlockchainDbContext>(options =>
                options.UseSqlite(connectionString));

            // Repository Pattern
            services.AddScoped<IBlockchainRecordRepository, BlockchainRecordRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // BlockCypher API Configuration
            services.Configure<BlockCypherOptions>(
                configuration.GetSection("BlockCypher"));

            // HTTP Client for BlockCypher
            services.AddHttpClient<BlockCypherClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            
            // Register implementation for IBlockCypherClient interface
            services.AddScoped<CIMarkets.Blockchain.Application.Commands.Handlers.IBlockCypherClient>(sp => sp.GetRequiredService<BlockCypherClient>());

            return services;
        }
    }
}
