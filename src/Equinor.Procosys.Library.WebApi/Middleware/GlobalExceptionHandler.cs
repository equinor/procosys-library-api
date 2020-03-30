using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Equinor.Procosys.Library.WebApi.Validation;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Equinor.Procosys.Library.WebApi.Middleware
{
    public class GlobalExceptionHandler
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(RequestDelegate next, ILogger<GlobalExceptionHandler> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                // Call the next delegate/middleware in the pipeline
                await _next(context);
            }
            catch (FluentValidation.ValidationException ve)
            {
                var response = new ValidationFailedResult(ve);

                _logger.LogInformation(response.Value.ToString());

                context.Response.StatusCode = (int)response.StatusCode;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync(JsonSerializer.Serialize(response.Value));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An exception occured");
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                context.Response.ContentType = "application/text";
                await context.Response.WriteAsync($"Something went wrong!");
            }
        }

        private class ValidationErrorResponse
        {
            public ValidationErrorResponse(int errorCount, IEnumerable<ValidationError> errors)
            {
                ErrorCount = errorCount;
                Errors = errors;
            }

            public int ErrorCount { get; }
            public IEnumerable<ValidationError> Errors { get; }
        }

        private class ValidationError
        {
            public ValidationError(string propertyName, string errorMessage, object attemptedValue)
            {
                PropertyName = propertyName;
                ErrorMessage = errorMessage;
                AttemptedValue = attemptedValue;
            }

            public string PropertyName { get; set; }
            public string ErrorMessage { get; set; }
            public object AttemptedValue { get; set; }
        }
    }
}
