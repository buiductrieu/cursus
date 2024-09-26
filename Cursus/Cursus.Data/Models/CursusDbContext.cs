using Cursus.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Data.Entities;

namespace Cursus.Data.Models
{
    public class CursusDbContext : IdentityDbContext<ApplicationUser>
    {
        public CursusDbContext()
        {
            
        }
        public CursusDbContext(DbContextOptions<CursusDbContext> options) : base(options)
        {

        }

        public virtual DbSet<Course> Courses { get; set; } = null!;
        public virtual DbSet<Category> Categories { get; set; } = null!;
        public virtual DbSet<Step> Steps { get; set; } = null!;
        public virtual DbSet<StepComment> StepComments { get; set; } = null!;
        public virtual DbSet<StepContent> StepContents { get; set; } = null!;
        public virtual DbSet<CourseVersion> CourseVersions { get; set; } = null!;
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public virtual DbSet<InstructorInfo> InstructorInfos { get; set; } = null!;
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
