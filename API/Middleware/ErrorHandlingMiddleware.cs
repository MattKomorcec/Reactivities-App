using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Application.Errors;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

/// This middleware is used for adding custom error handling middleware,
/// as I don't want the user to just get a 500 error in Production mode.
/// This way I can customize the error in any way I want to.
namespace API.Middleware
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _logger = logger;
            _next = next;

        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(context, ex);
            }
        }

        /// This method check to see what type of exception was raised,
        /// the custom RestException I created, or any other exception.
        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            object errors = null;

            switch (ex)
            {
                case RestException re:
                    _logger.LogError(ex, "REST ERROR");
                    errors = re.Errors;
                    context.Response.StatusCode = (int) re.Code;
                    break;
                case Exception e:
                    _logger.LogError(ex, "SERVER ERROR");
                    errors = string.IsNullOrWhiteSpace(e.Message) ? "Error" : e.Message;
                    context.Response.StatusCode = (int) HttpStatusCode.InternalServerError;
                    break;
            }

            context.Response.ContentType = "application/json";
            // If there were any errors, serialize them and write them to the response
            if (errors != null)
            {
                var result = JsonSerializer.Serialize(new
                {
                    errors
                });

                await context.Response.WriteAsync(result);
            }
        }
    }
}
