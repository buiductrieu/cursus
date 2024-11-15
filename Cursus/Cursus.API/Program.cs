

﻿using Cursus.Common.Helper;
using Cursus.Common.Middleware;

using Cursus.Data.Entities;
using Cursus.Data.Models;
using Cursus.Repository;
using Cursus.Service;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Cursus.Repository.Repository;
using Cursus.RepositoryContract.Interfaces;
using Demo_PayPal.Service;
using System.Threading.RateLimiting;
using Cursus.Service.Services;
using Scalar.AspNetCore;

namespace Cursus.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var fullVersion = Assembly.GetExecutingAssembly()
                                      .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                                      .InformationalVersion ?? "1.0.0";

            // Trim the commit hash suffix if it exists
            var version = fullVersion.Split('+')[0];



            // Add logging
            builder.Logging.ClearProviders();
            builder.Logging.AddConsole();

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

            // Add Rate Limit
            builder.Services.AddRateLimiter(option =>
            {
                option.AddFixedWindowLimiter("default", c =>
                {
                    c.Window = TimeSpan.FromHours(1);
                    c.PermitLimit = 1000;
                    c.QueueLimit = 1000;
                    c.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
                });

                option.OnRejected = async (context, cancellationToken) =>
                {
                    context.HttpContext.Response.StatusCode = 429;
                    await context.HttpContext.Response.WriteAsync("Rate limit exceeded. Please try again later.", cancellationToken);
                };
            });

            builder.Services.AddControllers();
            builder.Services.AddAutoMapper(typeof(MappingProfile));

            // Configure Swagger services
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSwaggerGen(opt =>
            {
                opt.SwaggerDoc("swagger",
                    new OpenApiInfo
                    {
                        Title = "Cursus API - "+ version,
                        Version = version
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
                //dbContext.Database.Migrate();
            }

            // Configure the HTTP request pipeline.
            app.UseSwagger(options =>
            {
                options.RouteTemplate = "/openapi/{documentname}.json";
            });
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/openapi/swagger.json", "Cursus API");
                c.RoutePrefix = string.Empty;
            });
            app.MapScalarApiReference();

            app.UseRateLimiter();

            app.UseExceptionHandler("/error");

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
