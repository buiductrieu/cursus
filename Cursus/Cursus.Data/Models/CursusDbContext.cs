using Cursus.Data.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Models
{
    public class CursusDbContext : IdentityDbContext<ApplicationUser, IdentityRole, string>
    {
        public CursusDbContext(DbContextOptions<CursusDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            var studentId = "3f98d917-eca4-4813-98ab-b17810eda3dc";
            var adminId = "f58c280f-f3b4-4b1f-903c-485b33cf3490";
            var instructorId = "f5a5e287-ac21-4ba5-be8d-353df9689a8e";

            var roles = new List<IdentityRole<Guid>>
            {
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse(studentId),
                    ConcurrencyStamp = studentId,
                    Name = "Student",
                    NormalizedName = "STUDENT"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse(adminId),
                    ConcurrencyStamp = adminId,
                    Name = "Admin",
                    NormalizedName = "ADMIN"
                },
                new IdentityRole<Guid>
                {
                    Id = Guid.Parse(instructorId),
                    ConcurrencyStamp = instructorId,
                    Name = "Instructor",
                    NormalizedName = "INSTRUCTOR"
                }
            };

            builder.Entity<IdentityRole<Guid>>().HasData(roles);
        }
    }
}
