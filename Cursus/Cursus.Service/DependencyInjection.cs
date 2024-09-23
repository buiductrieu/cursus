using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Cursus.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            // DI Service
            services.AddTransient<ICategoryService, CategoryService>();

            return services;
        }
    }
}
