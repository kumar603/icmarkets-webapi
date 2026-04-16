/*
 * File: Dockerfile
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Docker configuration for containerizing the CI Markets Blockchain API
 * 
 * Usage: This Dockerfile creates a Docker image for the .NET 8 Web API.
 *        Used for local deployment, CI/CD pipelines, and container orchestration.
 *        Supports multi-stage build for optimized image size.
 *        Can be run with: docker build -t cimarkets-api . && docker run -p 5000:5000 cimarkets-api
 * 
 * Dependencies: Docker, .NET 8 runtime
 */

# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project files
COPY ["CIMarkets.Blockchain.API/CIMarkets.Blockchain.API.csproj", "CIMarkets.Blockchain.API/"]
COPY ["CIMarkets.Blockchain.Application/CIMarkets.Blockchain.Application.csproj", "CIMarkets.Blockchain.Application/"]
COPY ["CIMarkets.Blockchain.Domain/CIMarkets.Blockchain.Domain.csproj", "CIMarkets.Blockchain.Domain/"]
COPY ["CIMarkets.Blockchain.Infrastructure/CIMarkets.Blockchain.Infrastructure.csproj", "CIMarkets.Blockchain.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "CIMarkets.Blockchain.API/CIMarkets.Blockchain.API.csproj"

# Copy source code
COPY . .

# Build application
WORKDIR "/src/CIMarkets.Blockchain.API"
RUN dotnet build "CIMarkets.Blockchain.API.csproj" -c Release -o /app/build

# Stage 2: Publish
FROM build AS publish
RUN dotnet publish "CIMarkets.Blockchain.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Stage 3: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app

# Copy published files from publish stage
COPY --from=publish /app/publish .

# Create logs directory
RUN mkdir -p /app/logs

# Expose ports
EXPOSE 5000
EXPOSE 5001

# Set environment variables
ENV ASPNETCORE_URLS=http://+:5000
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=3s --start-period=5s --retries=3 \
    CMD dotnet /app/CIMarkets.Blockchain.API.dll --health-check || exit 1

# Run application
ENTRYPOINT ["dotnet", "CIMarkets.Blockchain.API.dll"]
