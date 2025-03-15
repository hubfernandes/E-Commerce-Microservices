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
            var signInServie = context.Request.Headers["Api-GetWay"];
            if (signInServie.FirstOrDefault() is null)
            {
                context.Response.StatusCode = StatusCodes.Status503ServiceUnavailable;
                _logger.LogWarning("Unavailable Service");
                context.Response.WriteAsync("UnAvailable Service");
                return;
            }
            else
            {
                await _next(context);
            }
        }
    }
}
