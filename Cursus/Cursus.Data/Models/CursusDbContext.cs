using Cursus.Data.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Cursus.Data.Entities;
using Microsoft.AspNetCore.Identity;

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
        public virtual DbSet<CourseProgress> CourseProgresses { get; set; } = null!;
        public virtual DbSet<ApplicationUser> ApplicationUsers { get; set; } = null!;
        public virtual DbSet<InstructorInfo> InstructorInfos { get; set; } = null!;
        public virtual DbSet<CourseComment> CourseComments { get; set; } = null!;
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; } = null!;
        public virtual DbSet<Cart> Cart { get; set; } = null!;
        public virtual DbSet<CartItems> CartItems { get; set; } = null!;
        public virtual DbSet<Order> Order { get; set; } = null!;
        public virtual DbSet<Transaction> Transactions { get; set; } = null!;

        public virtual DbSet<Bookmark> Bookmarks { get; set; } = null!;
        public virtual DbSet<AdminComment> AdminComments { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<IdentityRole>().HasData(
                 new IdentityRole
                 {
                     Id = Guid.NewGuid().ToString(),
                     Name = "Admin",
                     NormalizedName = "ADMIN"
                 },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "Instructor",
                    NormalizedName = "INSTRUCTOR"
                },
                new IdentityRole
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "User",
                    NormalizedName = "USER"
                }
            );
            modelBuilder.Entity<AdminComment>()
                .HasOne(c => c.Commenter)
                .WithMany()
                .HasForeignKey(c => c.CommenterId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
