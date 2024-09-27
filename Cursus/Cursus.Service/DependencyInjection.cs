using Cursus.Common.Helper;
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
            services.AddTransient<IEmailService, EmailService>();


            services.AddScoped<APIResponse>();

            return services;
        }
    }
}
