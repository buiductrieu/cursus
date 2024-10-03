using Cursus.Common.Helper;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Common.Middleware
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        {
            services.AddExceptionHandler<UnauthorizedExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>();          
            return services;
        }
    }
}
