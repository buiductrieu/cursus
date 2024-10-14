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
using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
using Demo_PayPal.Service;

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


            var paypalSettings = builder.Configuration.GetSection("PayPal");
            builder.Services.Configure<PayPalSetting>(paypalSettings);

            // Đăng ký các dịch vụ
            builder.Services.AddScoped<PayPalClient>().AddHostedService<TransactionMonitoringService>();

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<CursusDbContext>()
                .AddDefaultTokenProviders();
            
            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("v1",
                    new OpenApiInfo
                    {
                        Title = "Cursus API - V1",
                        Version = "v1"
                    }
                 );
                opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter token",
                    Name = "Authorization",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });

                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type=ReferenceType.SecurityScheme,
                                    Id="Bearer"
                                }
                            },
                            new string[]{}
                        }
                    });

                opt.IncludeXmlComments(Assembly.GetExecutingAssembly());
               
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
