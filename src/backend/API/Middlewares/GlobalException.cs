using ApplicationService.Common.Exceptions;
using DomainLogic.Exceptions;
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
            var (errorStatusCode, errorMessage) = (ex) switch
            {
                DomainLogicException domEx => (domEx.ErrorCode) switch
                {
                    _ => (StatusCodes.Status422UnprocessableEntity, domEx.Details)
                },
                ApplicationServiceException appEx => (appEx.Code) switch
                {
                    ApplicationServiceErrorCode.MissingOrInvalidData => (StatusCodes.Status400BadRequest, appEx.Message),
                    ApplicationServiceErrorCode.DataNotFound => (StatusCodes.Status404NotFound, appEx.Message),
                    ApplicationServiceErrorCode.IdempotencyConflict => (StatusCodes.Status409Conflict, appEx.Message),
                    ApplicationServiceErrorCode.IdempotencyInvalid => (StatusCodes.Status400BadRequest, appEx.Message),
                    _ => (StatusCodes.Status500InternalServerError, "The problem was sent to the IT Department for help")
                },
                _ => (StatusCodes.Status500InternalServerError, "The problem was sent to the IT Department for help")
            };

            var problemDetails = new ProblemDetails();

            problemDetails.Status = errorStatusCode;
            problemDetails.Title = "An exception occurred";
            problemDetails.Detail = errorMessage;

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = errorStatusCode;

            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}
