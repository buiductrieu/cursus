using Cursus.Common.Helper;
using Cursus.Data.Models;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Cursus.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            // DI Service
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IInstructorService, InstructorService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddScoped<APIResponse>();

            return services;
        }
    }
}
