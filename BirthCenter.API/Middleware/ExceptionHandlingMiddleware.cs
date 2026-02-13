using System.Text.Json;
using BirthCenter.Domain.Exceptions;

namespace BirthCenter.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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

        private async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            _logger.LogError(exception, "An error occurred: {Message}", exception.Message);

            var response = context.Response;
            response.ContentType = "application/json";

            var (statusCode, message) = exception switch
            {
                NotFoundException notFound => (notFound.StatusCode, notFound.Message),
                ValidationException validation => (validation.StatusCode, validation.Message),
                ArgumentException arg => (400, arg.Message),
                KeyNotFoundException key => (404, key.Message),
                _ => (500, "An internal server error occurred. Please try again later.")
            };

            response.StatusCode = statusCode;

            var errorResponse = new
            {
                StatusCode = statusCode,
                Message = message,
                Timestamp = DateTime.UtcNow
            };

            await response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }
    }
}