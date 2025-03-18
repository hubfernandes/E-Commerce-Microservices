using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Shared.Middleware
{
    public class ListenToOnlyApiGetWay
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ListenToOnlyApiGetWay(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            _logger.LogInformation("Middleware executed!");

            var signInService = context.Request.Headers["ApiGateWay"];

            if (string.IsNullOrEmpty(signInService))
            {
                _logger.LogWarning("Request blocked! Missing 'ApiGateWay' header.");
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                await context.Response.WriteAsync("UnAvailable Service");
                return;
            }

            _logger.LogInformation("Request allowed! 'ApiGateWay' header found.");
            await _next(context);
        }
    }
}
