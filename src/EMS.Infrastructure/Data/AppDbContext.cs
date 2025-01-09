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

            modelBuilder.Entity<Department>()
                .HasOne(d => d.Manager)
                .WithOne()
                .HasForeignKey<Department>(d => d.ManagerId)
                .OnDelete(DeleteBehavior.SetNull);
            modelBuilder.Entity<Department>()
                .Property(d => d.Budget)
                .HasColumnType("decimal(18,2)");
            modelBuilder.Entity<Employee>()
                .HasOne(d => d.Department)
                .WithMany(d => d.Employees)
                .HasForeignKey(d => d.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction);
            modelBuilder.Entity<PerformanceReview>()
                .HasOne(e => e.Employee)
                .WithMany(e => e.PerformanceReviews)
                .HasForeignKey(e => e.EmployeeId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Employee>()
                .HasQueryFilter(p => !p.IsDeleted);
            modelBuilder.Entity<PerformanceReview>()
                .HasQueryFilter(p => !p.IsDeleted);

            modelBuilder.Entity<PerformanceReview>()
                .ToTable(p => p.HasCheckConstraint("CK_ReviewScore", "[ReviewScore] >= 1 AND [ReviewScore] <= 10"));

        }

        public override int SaveChanges()
        {
            var entries = ChangeTracker.Entries()
                .Where(e => e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified));

            foreach (var entry in entries)
            {
                if (entry.Entity is AuditableEntity auditableEntity && entry.State != EntityState.Deleted)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        auditableEntity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                if (entry.Entity is AuditableEntityWithSoftDeletation softDelete && entry.State == EntityState.Deleted)
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
            }

            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var entries = ChangeTracker.Entries()
                .Where(e => (e.Entity is AuditableEntity && (e.State == EntityState.Added || e.State == EntityState.Modified)) || e.Entity is AuditableEntityWithSoftDeletation);

            foreach (var entry in entries)
            {
                if (entry.Entity is AuditableEntity auditableEntity && entry.State != EntityState.Deleted)
                {
                    if (entry.State == EntityState.Added)
                    {
                        auditableEntity.CreatedAt = DateTime.UtcNow;
                    }
                    if (entry.State == EntityState.Modified)
                    {
                        auditableEntity.UpdatedAt = DateTime.UtcNow;
                    }
                }

                if (entry.Entity is AuditableEntityWithSoftDeletation softDelete && entry.State == EntityState.Deleted)
                {
                    softDelete.IsDeleted = true;
                    softDelete.DeletedAt = DateTime.UtcNow;
                    entry.State = EntityState.Modified;
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}
