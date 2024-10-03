using Microsoft.Extensions.DependencyInjection;

namespace Cursus.Common.Middleware
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddExceptionHandler(this IServiceCollection services)
        {
            services.AddExceptionHandler<UnauthorizedExceptionHandler>();
            services.AddExceptionHandler<GlobalExceptionHandler>(); 
            services.AddExceptionHandler<KeyNotFoundExceptionHandler>();
            services.AddExceptionHandler<NotImplementExceptionHandler>();
            services.AddExceptionHandler<BadRequestExceptionHandler>();
            return services;
        }
    }
}
