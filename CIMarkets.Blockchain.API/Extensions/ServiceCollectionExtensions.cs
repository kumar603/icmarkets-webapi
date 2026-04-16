/*
 * File: ServiceCollectionExtensions.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Dependency Injection and service configuration extensions
 * 
 * Usage: Extension methods for IServiceCollection and WebApplicationBuilder.
 *        Configures Application Services (MediatR, AutoMapper, Validators),
 *        CORS policies, structured logging with Serilog, and health checks.
 * 
 * Methods:
 *   AddApplicationServices() - Register MediatR, AutoMapper, FluentValidation
 *   AddCorsPolicy() - Configure CORS for cross-origin requests
 *   AddLogging() - Setup Serilog with console and file sinks
 * 
 * Dependencies: MediatR, AutoMapper, FluentValidation, Serilog,
 *              Microsoft.AspNetCore.Builder, Microsoft.Extensions.DependencyInjection
 */

using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace CIMarkets.Blockchain.API.Extensions
{
    /// <summary>
    /// Application services registration extension.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // Validation
            services.AddValidatorsFromAssemblyContaining(typeof(ServiceCollectionExtensions));

            // MediatR (CQRS)
            services.AddMediatR(config =>
            {
                config.RegisterServicesFromAssemblies(
                    typeof(Application.Commands.FetchAndStoreCommand).Assembly);
            });

            // AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            return services;
        }

        /// <summary>
        /// Configure CORS policies for the API.
        /// </summary>
        public static IServiceCollection AddCorsPolicy(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("AllowAll", policyBuilder =>
                {
                    policyBuilder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
            });

            return services;
        }

        /// <summary>
        /// Configure Serilog structured logging.
        /// </summary>
        public static WebApplicationBuilder AddLogging(this WebApplicationBuilder builder)
        {
            builder.Host.UseSerilog((context, configuration) =>
            {
                configuration
                    .MinimumLevel.Information()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "logs/blockchain-api-.txt",
                        rollingInterval: Serilog.RollingInterval.Day,
                        outputTemplate: "[{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz}] [{Level:u3}] {Message:lj}{NewLine}{Exception}");
            });

            return builder;
        }
    }
}
