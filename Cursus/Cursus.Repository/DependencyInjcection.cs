using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
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
            // DI Repository
            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IAdminRepository, AdminRepository>();
            services.AddTransient<IUserRepository, UserRepository>();
            

            services.AddTransient<ICourseRepository, CourseRepository>();
			services.AddTransient<IStepRepository, StepRepository>();
			// DI UnitOfWork
			services.AddTransient<IUnitOfWork, UnitOfWork>();
            services.AddTransient<ICourseRepository, CourseRepository>();
            services.AddTransient<IProgressRepository, ProgressRepository>();
            // DI UnitOfWork
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            return services;
        }
    }
}
