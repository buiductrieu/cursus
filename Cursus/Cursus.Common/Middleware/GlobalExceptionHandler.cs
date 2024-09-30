using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cursus.Common.Middleware
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public GlobalExceptionHandler()
        {
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var details = new ProblemDetails()
            {
                Detail = $"An error occurred: {exception.Message}",
                Instance = "API",
                Status = StatusCodes.Status500InternalServerError,
                Title = "Internal Server Error",
                Type = "https://httpstatuses.com/500"
            };
            
            var response = JsonSerializer.Serialize(details);

            httpContext.Response.ContentType = "application/json";

            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

            await httpContext.Response.WriteAsync(response, cancellationToken);

            return true;

        }
    }
}
