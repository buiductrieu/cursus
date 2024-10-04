using Cursus.Common.Helper;
using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.Repository;
using Cursus.Service;
using Cursus.ServiceContract;
using Cursus.Service.Services;
using Cursus.ServiceContract.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Cursus.Common.Middleware;
using System.Reflection;

namespace Cursus.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddRepository().AddService();

            builder.Services.AddExceptionHandler();

            builder.Services.AddProblemDetails();

            builder.Services.Configure<EmailSetting>(builder.Configuration.GetSection("EmailSettings"));
            // Add services to the container.
            builder.Services.AddDbContext<CursusDbContext>(options =>
               options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CursusDbContext>()
                .AddDefaultTokenProviders();
            
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "My API - V1",
                        Version = "v1"
                    }
                 );

                c.IncludeXmlComments(Assembly.GetExecutingAssembly());
               
            });






            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CursusDbContext>();
                dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.

            if (app.Environment.IsDevelopment())
            {
               

                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Cursus API v1");
                    c.RoutePrefix = string.Empty;
                });
            }
            
            app.UseExceptionHandler(_ => { });

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
