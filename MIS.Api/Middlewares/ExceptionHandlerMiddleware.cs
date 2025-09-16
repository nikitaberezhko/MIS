using System.Net;
using MIS.Api.Controllers.Contract.OpenApi.Models;

namespace MIS.Api.Middlewares;

public class ExceptionHandlerMiddleware(ILogger<ExceptionHandlerMiddleware> logger) : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Throw unhandled exception: {Message}", e.Message);
            await HandleExceptionAsync(context, e);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        
        var response = new Error
        {
            StatusCode = 0,
            Description = string.Empty,
            Timestamp = DateTime.UtcNow,
            TraceId = context.TraceIdentifier
        };

        switch (exception)
        {
            case ArgumentNullException nullEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Description = $"Required parameter is missing: {nullEx.Message}";
                break;
            
            case ArgumentException argEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Description = $"Invalid request parameters: {argEx.Message}";
                break;

            case UnauthorizedAccessException:
                response.StatusCode = (int)HttpStatusCode.Unauthorized;
                response.Description = "Insufficient permissions to perform the operation";
                break;
            
            case KeyNotFoundException:
                response.StatusCode = (int)HttpStatusCode.NotFound;
                response.Description = "Requested resource not found";
                break;
            
            case InvalidOperationException invalidOpEx:
                response.StatusCode = (int)HttpStatusCode.BadRequest;
                response.Description = $"Invalid operation: {invalidOpEx.Message}";
                break;
            
            case TimeoutException:
                response.StatusCode = (int)HttpStatusCode.RequestTimeout;
                response.Description = "Operation timeout exceeded";
                break;
            
            default:
                response.StatusCode = (int)HttpStatusCode.InternalServerError;
                response.Description = "Internal server error";
                break;
        }

        context.Response.StatusCode = response.StatusCode;

        var jsonResponse = response.ToJson();

        await context.Response.WriteAsync(jsonResponse);
    }
}