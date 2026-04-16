/*
 * File: DependencyInjection.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Dependency Injection setup for Infrastructure layer services
 * 
 * Usage: Extension method AddInfrastructure() registers:
 *        - BlockchainDbContext for EF Core data access
 *        - Repository and Unit of Work patterns (blockchain and payment)
 *        - BlockCypher API configuration and HTTP client
 *        - Payment gateway configuration and client
 *        Called from Program.cs to wire up infrastructure services.
 * 
 * Dependencies: All infrastructure services, BlockchainDbContext, repositories,
 *              BlockCypherClient, PaymentGatewayClient, BlockCypherOptions, 
 *              PaymentGatewayOptions, Microsoft.Extensions.DependencyInjection
 */

using CIMarkets.Blockchain.Application.Commands.Handlers;
using CIMarkets.Blockchain.Application.Services;
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

            // Repository Pattern - Blockchain
            services.AddScoped<IBlockchainRecordRepository, BlockchainRecordRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();

            // Repository Pattern - Payment
            services.AddScoped<IPaymentTransactionRepository, PaymentTransactionRepository>();

            // BlockCypher API Configuration
            services.Configure<BlockCypherOptions>(
                configuration.GetSection("BlockCypher"));

            // HTTP Client for BlockCypher
            services.AddHttpClient<BlockCypherClient>()
                .SetHandlerLifetime(TimeSpan.FromMinutes(5));
            
            // Register implementation for IBlockCypherClient interface
            services.AddScoped<CIMarkets.Blockchain.Application.Commands.Handlers.IBlockCypherClient>(sp => sp.GetRequiredService<BlockCypherClient>());

            // Payment Gateway Configuration
            services.Configure<PaymentGatewayOptions>(
                configuration.GetSection("PaymentGateway"));

            // Payment Gateway Client
            services.AddScoped<IPaymentGateway, PaymentGatewayClient>();

            return services;
        }
    }
}
