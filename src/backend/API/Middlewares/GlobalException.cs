using Microsoft.AspNetCore.Mvc;

namespace API.Middlewares
{
    public class GlobalException 
    {
        private readonly RequestDelegate _nextMiddleware;
        private readonly ILogger<GlobalException> _logger;

        public GlobalException(RequestDelegate nextMiddleware, ILogger<GlobalException> logger)
        {
            this._nextMiddleware = nextMiddleware;
            this._logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            // Do something before the next middleware

            try
            {
                await _nextMiddleware(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception ocurred...");
                await GenerateErrorResponse(context);
            }
        }

        private async Task GenerateErrorResponse(HttpContext context)
        {
            var problemDetails = new ProblemDetails();

            problemDetails.Title = "Unhandled exception ocurred";
            problemDetails.Detail = "The problem was sent to the IT Department for help";

            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
