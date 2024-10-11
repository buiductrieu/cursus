using Cursus.Common.Helper;
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
using System.Threading.RateLimiting;
using static Org.BouncyCastle.Math.EC.ECCurve;


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

            app.UseRateLimiter();

            app.UseExceptionHandler(_ => { });

            app.UseHttpsRedirection();

            app.UseAuthentication();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
