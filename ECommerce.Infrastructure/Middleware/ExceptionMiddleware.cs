using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using ECommerce.Application.Services.Logging;

namespace ECommerce.Infrastructure.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (DbUpdateException ex)
        {
            // Application Logger 
            var appLogger = context.RequestServices
                .GetRequiredService<IApplicationLogger<ExceptionMiddleware>>();

            var sqlEx = ex.InnerException as SqlException;

            if (sqlEx != null)
            {
                appLogger.LogError(ex, $"SQL exception ({sqlEx.Number})");
            }
            else
            {
                appLogger.LogError(ex, "EF Core DbUpdateException (non-sql)");
            }

            // Logger ASP.NET classique 
            _logger.LogError(ex, "Database update exception occurred.");

            context.Response.ContentType = "application/json";

            if (sqlEx is not null)
            {
                var (statusCode, message) = sqlEx.Number switch
                {
                    2627 or 2601 => (StatusCodes.Status409Conflict, "Unique constraint violation."),
                    547 => (StatusCodes.Status409Conflict, "Foreign key constraint violation."),
                    515 => (StatusCodes.Status400BadRequest, "Cannot insert null value."),
                    _ => (StatusCodes.Status500InternalServerError, "A database error occurred.")
                };

                context.Response.StatusCode = statusCode;

                var payload = JsonSerializer.Serialize(new
                {
                    statusCode,
                    message
                });

                await context.Response.WriteAsync(payload);
                return;
            }

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = JsonSerializer.Serialize(new
            {
                statusCode = context.Response.StatusCode,
                message = "An error occurred while saving the entity changes."
            });

            await context.Response.WriteAsync(response);
        }
        catch (Exception ex)
        {
            // Application Logger
            var appLogger = context.RequestServices
                .GetRequiredService<IApplicationLogger<ExceptionMiddleware>>();

            appLogger.LogError(ex, "Unknown exception");

            // Logger ASP.NET 
            _logger.LogError(ex, "Unhandled exception occurred.");

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;

            var response = JsonSerializer.Serialize(new
            {
                statusCode = context.Response.StatusCode,
                message = ex.Message
            });

            await context.Response.WriteAsync(response);
        }
    }
}
