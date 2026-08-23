namespace LibraryManagementSystemApi.Middelwares
{
    using Domain.Exceptions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.Logging;
    using System.ComponentModel.DataAnnotations;
    using System.Text.Json;


    public class GlobalExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionMiddleware> _logger;
        private readonly IHostEnvironment _env;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public GlobalExceptionMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var (statusCode, errorCode) = MapException(exception);

            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                _logger.LogError(exception, "Unhandled exception on {Path}", context.Request.Path);
            }
            else
            {
                _logger.LogWarning(exception, "Handled exception on {Path}: {Message}", context.Request.Path, exception.Message);
            }

            var response = new ErrorResponse
            {
                ErrorCode = errorCode,
                Message = exception.Message,
                Details = _env.IsDevelopment() ? exception.StackTrace : null,
                TraceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsync(JsonSerializer.Serialize(response, JsonOptions));
        }

        private static (int StatusCode, string ErrorCode) MapException(Exception exception)
        {
            return exception switch
            {
                BadRequestException => (StatusCodes.Status400BadRequest, "BAD_REQUEST"),
                ValidationException => (StatusCodes.Status400BadRequest, "VALIDATION_ERROR"),
                NotFoundException => (StatusCodes.Status404NotFound, "NOT_FOUND"),
                ConflictException => (StatusCodes.Status409Conflict, "CONFLICT"),
                ForbiddenException => (StatusCodes.Status403Forbidden, "FORBIDDEN"),
                UnauthorizedException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED"),
                UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, "UNAUTHORIZED"),
                _ => (StatusCodes.Status500InternalServerError, "INTERNAL_SERVER_ERROR")
            };
        }
    }
}
