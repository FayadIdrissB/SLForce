using System.Net;
using System.Text.Json;

namespace slf_backend.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly IWebHostEnvironment _environment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IWebHostEnvironment environment)
    {
        _next = next;
        _logger = logger;
        _environment = environment;
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
        context.Response.ContentType = "application/json";
        var response = context.Response;

        var errorResponse = new ErrorResponse
        {
            Message = exception.Message
        };

        switch (exception)
        {
            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                errorResponse.StatusCode = (int)HttpStatusCode.Unauthorized;
                break;

            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                errorResponse.StatusCode = (int)HttpStatusCode.NotFound;
                break;

            case ArgumentException:
            case InvalidOperationException:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                errorResponse.StatusCode = (int)HttpStatusCode.BadRequest;
                break;

            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.StatusCode = (int)HttpStatusCode.InternalServerError;
                errorResponse.Message = _environment.IsDevelopment() 
                    ? exception.Message 
                    : "Une erreur interne s'est produite";
                
                if (_environment.IsDevelopment())
                {
                    errorResponse.StackTrace = exception.StackTrace;
                }
                break;
        }

        _logger.LogError(exception, "Erreur non gérée: {Message}", exception.Message);

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var json = JsonSerializer.Serialize(errorResponse, options);
        await response.WriteAsync(json);
    }
}

public class ErrorResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? StackTrace { get; set; }
}

