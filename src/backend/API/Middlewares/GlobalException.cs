using ApplicationService.Common.Exceptions;
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
                await GenerateErrorResponse(context, ex);
            }
        }

        private async Task GenerateErrorResponse(HttpContext context, Exception ex)
        {
            var statusCode = (ex) switch
            {
                ApplicationServiceException => StatusCodes.Status404NotFound,
                _ => StatusCodes.Status500InternalServerError
            };

            var detail = statusCode == StatusCodes.Status500InternalServerError ? "The problem was sent to the IT Department for help" : ex.Message;

            var problemDetails = new ProblemDetails();

            problemDetails.Status = statusCode;
            problemDetails.Title = "Unhandled exception ocurred";
            problemDetails.Detail = detail;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = statusCode;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
