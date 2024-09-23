using Cursus.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cursus.Data.Models
{
    public class CursusDbContext : IdentityDbContext<ApplicationUser>
    {
        public CursusDbContext(DbContextOptions<CursusDbContext> options) : base(options)
        {

        }

        public DbSet<Course> Courses { get; set; } = null!;
        public DbSet<Category> Categories { get; set; } = null!;
        public DbSet<Step> Steps { get; set; } = null!;
        public DbSet<StepComment> StepComments { get; set; } = null!;
        public DbSet<StepContent> StepContents { get; set; } = null!;
        public DbSet<CourseVersion> CourseVersions { get; set; } = null!;
        public DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    base.OnConfiguring(optionsBuilder);
        //}
    }
}
