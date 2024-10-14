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
            services.AddTransient<ITransactionRepository, TransactionRepository>();
            services.AddTransient<IStepCommentRepository, StepCommentRepository>();
            services.AddTransient<IBookmarkRepository, BookmarkRepository>();
            services.AddTransient<IStepRepository, StepRepository>();
			services.AddTransient<ICartRepository, CartRepository>();
			services.AddTransient<IOrderRepository, OrderRepository>();
			services.AddTransient<ICourseProgressRepository, CourseProgressRepository>();
            services.AddTransient<ICartItemsRepository, CartItemsRepository>();
            services.AddTransient<IOrderRepository, OrderRepository>();
            services.AddTransient<ICartRepository, CartRepository>();
            // DI UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
