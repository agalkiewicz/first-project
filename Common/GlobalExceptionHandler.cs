using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;

namespace Movies.Api.Common;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (exception is DbUpdateException dbUpdateException)
        {
            _logger.LogError(dbUpdateException, "Database update error occurred");

            var message = "A database error occurred while processing your request.";
            var statusCode = StatusCodes.Status409Conflict;

            var innerMessage = dbUpdateException.InnerException?.Message ?? string.Empty;

            // Check for unique constraint violation
            if (innerMessage.Contains("duplicate key", StringComparison.OrdinalIgnoreCase)
                || innerMessage.Contains("unique constraint", StringComparison.OrdinalIgnoreCase))
            {
                message = "A record with the same value already exists.";
            }
            // Check for foreign key violation
            else if (innerMessage.Contains("foreign key", StringComparison.OrdinalIgnoreCase))
            {
                message = "The operation failed due to a reference constraint. The referenced record does not exist or cannot be deleted.";
                statusCode = StatusCodes.Status400BadRequest;
            }

            httpContext.Response.StatusCode = statusCode;
            await Results.Problem(
                title: "Database Error",
                detail: message,
                statusCode: statusCode
            ).ExecuteAsync(httpContext);

            return true;
        }

        return false;
    }
}
