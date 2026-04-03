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
            _logger.LogError(exception, "Unhandled Error occurred");

            var stackTrace = exception.StackTrace;
            var source = exception.Source;

            var pd = new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Server Error",
                Detail = exception.Message,

                // 👇 ADD THIS FOR DEBUGGING
                Extensions =
        {
            ["stackTrace"] = stackTrace,
            ["source"] = source
        }
            };

            httpContext.Response.StatusCode = pd.Status.Value;

            await httpContext.Response.WriteAsJsonAsync(pd, cancellationToken);

            return true;

        }
    }
}
