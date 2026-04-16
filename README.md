"# CI Markets Blockchain API

A production-ready .NET 8 Web API for fetching and storing blockchain data from external APIs (BlockCypher).

## Overview

The API integrates with BlockCypher to fetch data for multiple blockchain types:
- **Bitcoin (BTC)**
- **Ethereum (ETH)**
- **Litecoin (LTC)**
- **Dash (DASH)**

Data is stored in SQLite with full JSON responses maintained for historical records.

## Architecture

Built using **Clean Architecture** with:
- **Domain Layer**: Core business entities and repository interfaces
- **Application Layer**: CQRS pattern with MediatR handlers
- **Infrastructure Layer**: Database context, repositories, and external API clients
- **API Layer**: RESTful HTTP endpoints

### Key Patterns
- ✅ **Repository Pattern** - Abstract data access
- ✅ **CQRS** - Separate read/write operations
- ✅ **Dependency Injection** - Loose coupling
- ✅ **Async/Await** - Non-blocking operations
- ✅ **Structured Logging** - Serilog integration
- ✅ **Global Error Handling** - Middleware-based exception handling
- ✅ **Health Checks** - Database and API health endpoints
- ✅ **Parallel Execution** - Task.WhenAll for concurrent operations

## Project Structure

```
CIMarkets.Blockchain.sln
├── CIMarkets.Blockchain.Domain              # Entities, enums, interfaces
├── CIMarkets.Blockchain.Application         # CQRS, validators, DTOs
├── CIMarkets.Blockchain.Infrastructure      # Database, repositories, API clients
├── CIMarkets.Blockchain.API                 # Controllers, middleware, configuration
└── tests/
    └── CIMarkets.Blockchain.Tests           # Unit and integration tests
```

## Quick Start

1. **Build solution:**
   ```bash
   dotnet build
   ```

2. **Set API key (environment variable or appsettings):**
   ```bash
   $env:BLOCKCYPHER_API_KEY = "your_key_here"
   ```

3. **Run API:**
   ```bash
   cd CIMarkets.Blockchain.API
   dotnet run
   ```

API available at: `https://localhost:7001`

## API Endpoints

### Fetch & Store Blockchain Data
**POST** `/api/v1/blockchain/sync`
```json
{ "blockchainType": 1, "address": null }
```

### Get History (Sorted by CreatedAt DESC)
**GET** `/api/v1/blockchain/history?blockchainType=1&pageNumber=1&pageSize=50`

### Health Check
**GET** `/health`

## Key Files

| File | Purpose |
|------|---------|
| `Domain/Entities/BlockchainRecord.cs` | Single entity for all blockchains |
| `Application/Commands/FetchAndStoreCommand.cs` | CQRS write operation |
| `Application/Queries/GetBlockchainHistoryQuery.cs` | CQRS read operation |
| `Infrastructure/Repositories/BlockchainRecordRepository.cs` | Data access |
| `Infrastructure/ExternalApis/BlockCypherClient.cs` | External API integration |
| `API/Controllers/BlockchainController.cs` | HTTP endpoints |
| `API/Program.cs` | Bootstrap & DI configuration |

## Design Decisions

✅ **Single BlockchainRecord Entity** - Simplicity over premature abstraction  
✅ **One Controller** - Routes on enum parameter  
✅ **Minimal CQRS** - Only essential commands/queries  
✅ **CreatedAt Only** - No UpdatedAt (not required)  
✅ **Task.WhenAll Pattern** - Parallel execution support  
✅ **Serilog Logging** - Structured logging with file + console  
✅ **CORS Support** - AllowAll policy configured  
✅ **Health Checks** - Database + API health monitoring  

## Configuration

**appsettings.json:**
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=blockchain.db"
  },
  "BlockCypher": {
    "BaseUrl": "https://api.blockcypher.com/v1",
    "ApiKey": "${BLOCKCYPHER_API_KEY}",
    "TimeoutSeconds": 30
  }
}
```

## Database

Single SQLite table with index for efficient queries:
```sql
CREATE TABLE BlockchainRecords (
    Id INTEGER PRIMARY KEY,
    BlockchainType INTEGER,
    RawJson TEXT,
    CreatedAt DATETIME DEFAULT CURRENT_TIMESTAMP
);
CREATE INDEX IX_BlockchainRecords_Type_CreatedAt 
    ON BlockchainRecords(BlockchainType, CreatedAt DESC);
```

## Testing

```bash
dotnet test
```

Example test: `CIMarkets.Blockchain.Tests/Application/Validators/FetchAndStoreValidatorTests.cs`

## Logging

Structured logs with Serilog:
- Console output (development)
- File output (daily rolling: `logs/blockchain-api-*.txt`)
- Structured format with context

## SOLID Principles

| Principle | Implementation |
|-----------|-----------------|
| **S** - Single Responsibility | One entity, one repository, one handler per operation |
| **O** - Open/Closed | New blockchains via enum, not code modification |
| **L** - Liskov Substitution | Concrete implementations of IBlockchainRecordRepository |
| **I** - Interface Segregation | Focused interfaces (IBlockCypherClient, IUnitOfWork) |
| **D** - Dependency Inversion | Inject abstractions, not implementations |

## Error Handling

Global middleware + structured exceptions:
```json
{
  "success": false,
  "message": "Error details",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

## License

© 2024 CI Markets. All rights reserved." 
