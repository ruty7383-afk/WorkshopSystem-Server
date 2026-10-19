using System.Net;
using WorkshopSystem.Application.Exceptions;

namespace WorkshopSystem.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
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
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(
        HttpContext context,
        Exception exception)
    {
        var correlationId =
            context.Response.Headers["X-Correlation-ID"].FirstOrDefault()
            ?? Guid.NewGuid().ToString();

        var statusCode = exception switch
        {
            NotFoundException =>
                StatusCodes.Status404NotFound,

            UnauthorizedException =>
                StatusCodes.Status401Unauthorized,

            ConflictException =>
                StatusCodes.Status409Conflict,

            BusinessException =>
                StatusCodes.Status409Conflict,

            _ =>
                StatusCodes.Status500InternalServerError
        };

        if (statusCode == StatusCodes.Status500InternalServerError)
        {
            _logger.LogError(
                exception,
                "Unexpected error. CorrelationId={CorrelationId}",
                correlationId);
        }
        else
        {
            _logger.LogWarning(
                exception,
                "Request failed with status {StatusCode}. CorrelationId={CorrelationId}",
                statusCode,
                correlationId);
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        var message =
            statusCode == StatusCodes.Status500InternalServerError
                ? "An unexpected error occurred."
                : exception.Message;

        await context.Response.WriteAsJsonAsync(new
        {
            statusCode,
            message,
            correlationId
        });
    }
}