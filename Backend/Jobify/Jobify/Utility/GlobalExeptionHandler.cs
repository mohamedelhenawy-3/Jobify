using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Jobify.Utility
{
    public class GlobalExeptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExeptionHandler> _logger;

        public GlobalExeptionHandler(ILogger<GlobalExeptionHandler> logger)
        {
            _logger = logger;
        }
        public async  ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError("Unhandled Error ", exception.Message);


            var pd = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Type = "https://datatracker.ietf.org/doc/html/rfc7231#section-6.6.1",
                Detail = exception.Message // In production, hide this detail for security!
            };
            httpContext.Response.StatusCode = pd.Status.Value;
            await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);

            return true; // Tells the system we handled the error

        }
    }
}
