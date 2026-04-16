# IMPLEMENTATION GUIDE

## Overview
This document outlines the implementation structure of the CI Markets Blockchain API built with Clean Architecture and CQRS patterns.

## 1. Domain Layer (Core Business Logic)

### Responsibility
- Define business entities
- Express domain logic
- Define repository and service interfaces
- NO dependencies on other layers

### Key Files
- `Entities/BlockchainRecord.cs` - Single entity storing blockchain data
- `Enums/BlockchainType.cs` - Supported blockchain types
- `Repositories/IBlockchainRecordRepository.cs` - Data access contract
- `Repositories/IUnitOfWork.cs` - Transaction management contract
- `Exceptions/` - Domain-specific exceptions

### Design Decision
- **Single Entity**: Uses `BlockchainRecord` instead of hierarchy (Bitcoin, Ethereum, etc.)
- **Reasoning**: Simplicity, common fields (Id, BlockchainType, RawJson, CreatedAt)
- **Future**: Can split into separate entities if specialized fields needed

---

## 2. Application Layer (CQRS & Use Cases)

### Responsibility
- Implement CQRS pattern
- Handle business orchestration
- Validate input (FluentValidation)
- Map DTOs

### CQRS Structure

#### Commands (Write Operations)
```
Commands/
├── FetchAndStoreCommand.cs
└── Handlers/
    └── FetchAndStoreCommandHandler.cs
```

**FetchAndStoreCommand**
- Input: BlockchainType + optional Address
- Output: CommandResult with stored record details

**FetchAndStoreCommandHandler**
- Calls BlockCypher API client
- Creates BlockchainRecord entity
- Uses Unit of Work pattern
- Returns result with DTO

#### Queries (Read Operations)
```
Queries/
├── GetBlockchainHistoryQuery.cs
└── Handlers/
    └── GetBlockchainHistoryQueryHandler.cs
```

**GetBlockchainHistoryQuery**
- Input: BlockchainType, PageNumber, PageSize
- Output: Paginated BlockchainHistoryResponse

**GetBlockchainHistoryQueryHandler**
- Queries repository
- Maps entities to DTOs
- Returns paginated results

### Validators
- Validates commands/queries before execution
- FluentValidation package

### DTOs
- **FetchAndStoreRequest**: Input for POST /sync
- **BlockchainRecordResponse**: Single record representation
- **BlockchainHistoryResponse**: Paginated history collection

---

## 3. Infrastructure Layer (Data Access & External APIs)

### Responsibility
- Implement data persistence
- External API integration
- Dependency registration

### Database (`Data/`)
```
Data/
├── BlockchainDbContext.cs        # EF Core DbContext
├── Configurations/
│   └── BlockchainRecordConfiguration.cs  # Entity mapping
└── Migrations/
    └── (EF Core migrations here)
```

**BlockchainDbContext**
- SQLite-based
- Single table: BlockchainRecords

**BlockchainRecordConfiguration**
- Fluent API configuration
- Table schema definition
- Index creation (BlockchainType + CreatedAt DESC)

### Repositories (`Repositories/`)
```
Repositories/
├── BlockchainRecordRepository.cs  # Implements IBlockchainRecordRepository
└── UnitOfWork.cs                  # Implements IUnitOfWork
```

**BlockchainRecordRepository**
- Implements domain interface
- LINQ queries to DbContext
- Methods: GetByIdAsync, GetHistoryAsync, AddAsync, SaveChangesAsync

**UnitOfWork**
- Lazy-loads repository instances
- Manages transactions (CommitAsync, RollbackAsync)

### External APIs (`ExternalApis/`)
```
ExternalApis/
├── BlockCypherClient.cs           # Implementation
└── (IBlockCypherClient - defined in Application layer)
```

**BlockCypherClient**
- Fetches data from BlockCypher API
- Methods for each blockchain: FetchBitcoinAsync, FetchEthereumAsync, FetchLitecoinAsync, FetchDashAsync
- Error handling: ExternalApiException
- Timeout configuration
- Logging integration

### Configuration (`Configuration/`)
```
Configuration/
├── BlockchainOptions.cs  # Configuration POCO
├── DependencyInjection.cs  # Service registration
```

**BlockchainOptions**
- Binds to appsettings.json "BlockCypher" section
- BaseUrl, ApiKey, TimeoutSeconds

**DependencyInjection**
- Registers DbContext
- Registers repositories
- Registers HTTP client with BlockCypherClient
- Registers configuration

---

## 4. API Layer (HTTP Endpoints)

### Responsibility
- RESTful endpoints
- Request routing
- Minimal business logic

### Controllers (`Controllers/`)
```
Controllers/
└── BlockchainController.cs
    ├── POST /api/v1/blockchain/sync
    └── GET /api/v1/blockchain/history
```

**BlockchainController**
- Single controller for all blockchain operations
- Delegates to MediatR handlers (CQRS)
- Logging per operation
- Returns appropriate status codes

### Middleware (`Middleware/`)
```
Middleware/
└── ErrorHandlingMiddleware.cs
```

**ErrorHandlingMiddleware**
- Catches all unhandled exceptions
- Returns standardized error response
- Logs errors with context

### Extensions (`Extensions/`)
```
Extensions/
└── ServiceCollectionExtensions.cs
```

**ServiceCollectionExtensions**
- `AddApplicationServices`: Registers MediatR, Validators, AutoMapper
- `AddCorsPolicy`: CORS configuration
- `AddLogging`: Serilog setup

### Configuration Files
- **appsettings.json**: Production configuration
- **appsettings.Development.json**: Development overrides

### Program.cs
Complete bootstrap:
1. Logging setup (Serilog)
2. Service registration
3. Middleware pipeline
4. Database initialization

---

## 5. Integration Points

### Request Flow (FetchAndStore)
```
Controller.FetchAndStoreAsync(request)
  ↓
Create FetchAndStoreCommand
  ↓
MediatR.Send(command)
  ↓
Validator.ValidateAsync(command)
  ↓
FetchAndStoreCommandHandler.Handle(command)
  ├─→ BlockCypherClient.FetchBitcoinAsync()
  ├─→ Create BlockchainRecord entity
  ├─→ UnitOfWork.BlockchainRecords.AddAsync()
  ├─→ UnitOfWork.CommitAsync()
  └─→ Return CommandResult with DTO
  ↓
Controller returns OK(result)
```

### Request Flow (GetHistory)
```
Controller.GetHistoryAsync(blockchainType, pageNumber, pageSize)
  ↓
Create GetBlockchainHistoryQuery
  ↓
MediatR.Send(query)
  ↓
Validator.ValidateAsync(query)
  ↓
GetBlockchainHistoryQueryHandler.Handle(query)
  ├─→ Repository.GetHistoryAsync()
  ├─→ Map entities to DTOs
  └─→ Return BlockchainHistoryResponse
  ↓
Controller returns OK(result)
```

### Dependency Injection Chain
```
Program.cs
├─→ AddInfrastructure(configuration)
│   ├─→ AddDbContext<BlockchainDbContext>
│   ├─→ AddScoped<IBlockchainRecordRepository, BlockchainRecordRepository>
│   ├─→ AddScoped<IUnitOfWork, UnitOfWork>
│   ├─→ Configure<BlockchainOptions>
│   └─→ AddHttpClient<IBlockCypherClient, BlockCypherClient>
├─→ AddApplicationServices
│   ├─→ AddValidatorsFromAssembly
│   ├─→ AddMediatR
│   └─→ AddAutoMapper
└─→ AddCorsPolicy
```

---

## 6. Parallel Execution (As Required)

### Task.WhenAll Usage
In `FetchAndStoreCommandHandler.FetchBlockchainDataAsync()`:

```csharp
var tasks = new List<Task<string>>();

// Add tasks to fetch data
switch (request.BlockchainType)
{
    case BlockchainType.Bitcoin:
        tasks.Add(_blockCypherClient.FetchBitcoinAsync(...));
        break;
    // ... other cases
}

// Execute all tasks in parallel
var results = await Task.WhenAll(tasks);
return results.First();
```

**Benefits**:
- Parallel execution if multiple data sources needed
- Efficient use of async/await
- Better performance for concurrent operations

---

## 7. Logging Strategy

### Serilog Configuration
- **Console Sink**: Real-time console output
- **File Sink**: Daily rolling logs to `logs/` directory
- **Structured Logging**: Includes context (timestamps, log levels)

### Log Points
- API startup
- Command execution
- Query execution
- External API calls
- Record storage
- Errors and exceptions

### Log Format
```
[2024-01-15 10:30:45.123 +00:00] [INF] Message with {ContextInfo}
```

---

## 8. Database Schema

### Single Table Design
```sql
BlockchainRecords
├── Id (PK, INT)
├── BlockchainType (INT) → Enum (1=BTC, 2=ETH, 3=LTC, 4=DASH)
├── RawJson (TEXT)
└── CreatedAt (DATETIME, indexed)
```

### Indexing Strategy
- **Primary Index**: ID (auto-generated)
- **Query Index**: (BlockchainType, CreatedAt DESC) for efficient history retrieval

---

## 9. Error Handling

### Exception Hierarchy
- **BlockchainDataException**: Domain-level errors
- **ExternalApiException**: API integration errors
- **Validation Exceptions**: Input validation failures

### Middleware Response
```json
{
  "success": false,
  "message": "Error description",
  "timestamp": "2024-01-15T10:30:00Z"
}
```

---

## 10. Next Steps to Extend

### Add Authentication
```csharp
// In Program.cs
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(...);
app.UseAuthentication();
```

### Add Caching
```csharp
services.AddStackExchangeRedisCache(...);
```

### Add Rate Limiting
```csharp
app.UseRateLimiter();
```

### Add API Versioning
```csharp
services.AddApiVersioning();
```

---

This structure is production-ready, maintainable, and follows SOLID principles throughout.
