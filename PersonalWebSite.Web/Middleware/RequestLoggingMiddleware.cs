using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace PersonalWebSite.Web.Middleware
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                var startTime = DateTime.UtcNow;
                _logger.LogInformation($"Request started: {context.Request.Method} {context.Request.Path}");

                await _next(context);

                var duration = DateTime.UtcNow - startTime;
                _logger.LogInformation($"Request completed: {context.Request.Method} {context.Request.Path} - Status: {context.Response.StatusCode} - Duration: {duration.TotalMilliseconds}ms");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"An error occurred while processing the request: {context.Request.Method} {context.Request.Path}");
                throw;
            }
        }
    }

    public static class RequestLoggingMiddlewareExtensions
    {
        public static IApplicationBuilder UseRequestLogging(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<RequestLoggingMiddleware>();
        }
    }
} 