using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Mvc.ViewComponents;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Repository
{
    public static class DependencyInjcection
    {
        public static IServiceCollection AddRepository(this IServiceCollection services)
        {
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IInstructorInfoRepository, InstructorRepository>();
            services.AddTransient<IAdminRepository, AdminRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            services.AddTransient<IStepContentRepository, StepContentRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
			services.AddTransient<IStepRepository, StepRepository>();
            services.AddTransient<ICourseCommentRepository, CourseCommentRepository>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<IProgressRepository, ProgressRepository>();
            services.AddTransient<IRefreshTokenRepository, RefreshTokenRepository>();
            services.AddTransient<IStepCommentRepository, StepCommentRepository>();
            services.AddTransient<IBookmarkRepository, BookmarkRepository>();
            services.AddTransient<IStepRepository, StepRepository>();
            services.AddTransient<ICartRepository, CartRepository>();
            services.AddTransient<ICartItemsRepository, CartItemsRepository>();// DI UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
