using Cursus.Data.Entities;
using Cursus.Data.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Cursus.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddDbContext<CursusDbContext>(options =>
            options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<CursusDbContext>()
            .AddDefaultTokenProviders();
            builder.Services.AddIdentity<ApplicationUser, IdentityRole<Guid>>(options =>
            {
                options.Password.RequiredLength = 12;
                options.Password.RequireNonAlphanumeric = true;
                options.Password.RequireDigit = true; 
                options.Password.RequireUppercase = true;
                options.User.RequireUniqueEmail = true; // Yêu c?u email ph?i duy nh?t
                options.SignIn.RequireConfirmedEmail = false; // Yêu c?u xác nh?n email khi ??ng nh?p
            })
            .AddEntityFrameworkStores<CursusDbContext>()
            .AddDefaultTokenProviders();



            builder.Services.AddControllers();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<CursusDbContext>();
                dbContext.Database.Migrate(); 
            }

            // Configure the HTTP request pipeline.

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.UseAuthentication();

            app.MapControllers();

            app.Run();
        }
    }
}
