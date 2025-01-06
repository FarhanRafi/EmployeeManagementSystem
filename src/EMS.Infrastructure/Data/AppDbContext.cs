using EMS.Domain.Common;
using EMS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EMS.Infrastructure.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<PerformanceReview> PerformanceReviews { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SoftDelete>().HasNoKey();

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithOne()
                .HasForeignKey<Department>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Employee>()
                .HasOne(d => d.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<PerformanceReview>()
                .HasOne(e => e.Employee)
                .WithMany(e => e.PerformanceReviews)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.SetNull);


            modelBuilder.Entity<Employee>()
                .HasQueryFilter(p => !p.SoftDelete.IsDeleted);
            modelBuilder.Entity<PerformanceReview>()
                .HasQueryFilter(p => !p.SoftDelete.IsDeleted);

            modelBuilder.Entity<PerformanceReview>()
                .ToTable(p => p.HasCheckConstraint("CK_ReviewScore", "[ReviewScore] BETWEEN 1 AND 10"));

        }
    }
}
