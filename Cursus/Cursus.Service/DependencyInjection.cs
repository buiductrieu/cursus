using Cursus.Common.Helper;
using Cursus.Data.Models;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Demo_PayPal.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace Cursus.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddService(this IServiceCollection services)
        {
            // DI Services
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<IEmailService, EmailService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<ICourseService, CourseService>();
            services.AddTransient<ICourseProgressService, CourseProgressService>();         
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IInstructorService, InstructorService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddScoped<APIResponse>();
            services.AddTransient<IAdminService, AdminService>();
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IStepContentService, StepContentService>();
            services.AddTransient<IAzureBlobStorageService, AzureBlobStorageService>();
           
            services.AddTransient<IStepCommentService, StepCommentService>();

            services.AddTransient<ICourseCommentService, CourseCommentService>();   
            services.AddTransient<IPaymentService,PaymentService>();
            return services;
        }
    }
}
