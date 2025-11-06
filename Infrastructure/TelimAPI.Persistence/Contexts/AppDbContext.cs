using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Contexts
{
    public class AppDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Training> Trainings { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<TrainingParticipant> TrainingParticipants { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
            // Training Court (Many-to-Many)
            modelBuilder.Entity<TrainingCourt>()
                .HasKey(tc => new { tc.TrainingId, tc.CourtId });

            modelBuilder.Entity<TrainingCourt>()
                .HasOne(tc => tc.Training)
                .WithMany(t => t.TrainingCourts)
                .HasForeignKey(tc => tc.TrainingId);

            modelBuilder.Entity<TrainingCourt>()
                .HasOne(tc => tc.Court)
                .WithMany(c => c.TrainingCourts)
                .HasForeignKey(tc => tc.CourtId);


            // Training Department (Many-to-Many)
            modelBuilder.Entity<TrainingDepartment>()
                .HasKey(td => new { td.TrainingId, td.DepartmentId });

            modelBuilder.Entity<TrainingDepartment>()
                .HasOne(td => td.Training)
                .WithMany(t => t.TrainingDepartments)
                .HasForeignKey(td => td.TrainingId);

            modelBuilder.Entity<TrainingDepartment>()
                .HasOne(td => td.Department)
                .WithMany(d => d.TrainingDepartments)
                .HasForeignKey(td => td.DepartmentId);


            // User Training (Many-to-Many via TrainingParticipant)
            modelBuilder.Entity<TrainingParticipant>()
                .HasOne(tp => tp.User)
                .WithMany(u => u.TrainingParticipants)
                .HasForeignKey(tp => tp.UserId);

            modelBuilder.Entity<TrainingParticipant>()
                .HasOne(tp => tp.Training)
                .WithMany(t => t.Participants)
                .HasForeignKey(tp => tp.TrainingId);

            // Department Court (One-to-Many)
            modelBuilder.Entity<Department>()
                .HasOne(d => d.Court)
                .WithMany(c => c.Departments)
                .HasForeignKey(d => d.CourtId)
                 .OnDelete(DeleteBehavior.NoAction);

            // User Department (One-to-Many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany(d => d.Users)
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.NoAction);

            // User Court (One-to-Many)
            modelBuilder.Entity<User>()
                .HasOne(u => u.Court)
                .WithMany(c => c.Users)
                .HasForeignKey(u => u.CourtId)
                 .OnDelete(DeleteBehavior.NoAction);

            // TrainingFeedback TrainingParticipant (One-to-One)
            modelBuilder.Entity<TrainingFeedback>()
                .HasOne(tf => tf.TrainingParticipant)
                .WithOne()
                .HasForeignKey<TrainingFeedback>(tf => tf.TrainingParticipantId);

            //-------------------------------------------------------

           
            
        }


        // ===== Automatic CreatedDate update =====
        public override int SaveChanges()
        {
            UpdateTimestamps();
            return base.SaveChanges();
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            UpdateTimestamps();
            return await base.SaveChangesAsync(cancellationToken);
        }


        private void UpdateTimestamps()
        {
            var entries = ChangeTracker.Entries()
                // İndi yalnız BaseEntity olanları seçirik, həmçinin yoxlama sadələşdi
                .Where(e => e.Entity.GetType().IsSubclassOf(typeof(TelimAPI.Domain.Entities.Common.BaseEntity)) ||
                            e.Entity.GetType() == typeof(TelimAPI.Domain.Entities.Common.BaseEntity))
                .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified); // Added və ya Modified

            foreach (var entry in entries)
            {
                // 'as' istifadə edərək təhlükəsiz çevrilmə aparırıq
                if (entry.Entity is TelimAPI.Domain.Entities.Common.BaseEntity baseEntity)
                {
                    if (entry.State == EntityState.Added)
                    {
                        baseEntity.CreatedDate = DateTime.UtcNow;
                    }
                    // Modified state üçün də update yazmaq olar, lakin sualınızda yoxdur.
                }
            }
        }
    }
}
