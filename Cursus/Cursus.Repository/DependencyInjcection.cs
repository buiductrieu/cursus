using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
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

            return services;
        }
    }
}
