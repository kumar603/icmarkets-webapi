/*
 * File: ErrorHandlingMiddleware.cs
 * Author: Kiran Kumar
 * Date: April 16, 2026
 * Purpose: Global exception handling middleware for standardized error responses
 * 
 * Usage: Intercepts all unhandled exceptions in the request pipeline.
 *        Converts exceptions to standardized ErrorResponse JSON format.
 *        Logs errors with structured logging (Serilog) for audit trails.
 * 
 * Dependencies: ILogger, System.Net, System.Text.Json
 */

using System.Net;
using System.Text.Json;

namespace CIMarkets.Blockchain.API.Middleware
{
    /// <summary>
    /// Global exception handling middleware for API errors.
    /// </summary>
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;

        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";

            var response = new ErrorResponse
            {
                Success = false,
                Message = exception.Message,
                Timestamp = DateTime.UtcNow
            };

            context.Response.StatusCode = exception switch
            {
                ArgumentException => (int)HttpStatusCode.BadRequest,
                KeyNotFoundException => (int)HttpStatusCode.NotFound,
                UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,
                _ => (int)HttpStatusCode.InternalServerError
            };

            return context.Response.WriteAsJsonAsync(response);
        }
    }

    /// <summary>
    /// Standard error response format.
    /// </summary>
    public class ErrorResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public DateTime Timestamp { get; set; }
    }
}
