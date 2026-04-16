using CIMarkets.Blockchain.API.Extensions;
using CIMarkets.Blockchain.API.Middleware;
using CIMarkets.Blockchain.Infrastructure.Configuration;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ============================================
// 1. LOGGING (Serilog)
// ============================================
builder.AddLogging();

// ============================================
// 2. SERVICES & DEPENDENCY INJECTION
// ============================================

// API Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplicationServices();

// CORS
builder.Services.AddCorsPolicy();

// Health Checks
builder.Services.AddHealthChecks()
    .AddDbContextCheck<CIMarkets.Blockchain.Infrastructure.Data.BlockchainDbContext>("Database");

// Infrastructure (Database, Repositories, External APIs)
builder.Services.AddInfrastructure(builder.Configuration);

// ============================================
// 3. BUILD APPLICATION
// ============================================
var app = builder.Build();

// ============================================
// 4. MIDDLEWARE PIPELINE
// ============================================

// Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "CI Markets Blockchain API v1");
        c.RoutePrefix = string.Empty; // Serve swagger at root
    });
}

// HTTPS Redirect
app.UseHttpsRedirection();

// CORS
app.UseCors("AllowAll");

// Global Exception Handling
app.UseMiddleware<ErrorHandlingMiddleware>();

// Authentication & Authorization (if needed later)
app.UseAuthentication();
app.UseAuthorization();

// Health Checks Endpoint
app.MapHealthChecks("/health");

// API Routes
app.MapControllers();

// ============================================
// 5. DATABASE INITIALIZATION
// ============================================
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CIMarkets.Blockchain.Infrastructure.Data.BlockchainDbContext>();
    
    try
    {
        // Ensure database is created and migrations are applied
        dbContext.Database.EnsureCreated();
        app.Logger.LogInformation("Database initialized successfully");
    }
    catch (Exception ex)
    {
        app.Logger.LogError(ex, "Error initializing database");
        throw;
    }
}

// ============================================
// 6. STARTUP
// ============================================
app.Logger.LogInformation("Starting CI Markets Blockchain API");
app.Run();
