using Cursus.Common.Helper;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Cursus.Common.Middleware
{
    public class UnauthorizedExceptionHandler : IExceptionHandler
    {
        public UnauthorizedExceptionHandler()
        {

        }
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
           if (exception is UnauthorizedAccessException)
           {
                var details = new ProblemDetails()
                {
                    Detail = $"An error occurred: {exception.Message}",
                    Instance = "Request",
                    Status = StatusCodes.Status403Forbidden,
                    Title = "Unauthorized Access",
                    Type = "https://httpstatuses.com/403"
                };

                var response = JsonSerializer.Serialize(details);

                httpContext.Response.ContentType = "application/json";

                httpContext.Response.StatusCode = StatusCodes.Status403Forbidden;

                await httpContext.Response.WriteAsync(response, cancellationToken);
                return true;
           }
            return false;
        }
    }
}
