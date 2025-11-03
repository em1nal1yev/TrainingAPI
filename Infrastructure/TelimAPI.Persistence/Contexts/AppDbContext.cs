using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Contexts
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Training> Trainings { get; set; }
        public DbSet<Court> Courts { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<TrainingParticipant> TrainingParticipants { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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

            modelBuilder.Entity<Court>().HasData(
                new Court { Id = Guid.Parse("11111111-1111-1111-1111-111111111111"), Name = "Ali Məhkəmə" },
                new Court { Id = Guid.Parse("22222222-2222-2222-2222-222222222222"), Name = "Apellyasiya Məhkəməsi" },
                new Court { Id = Guid.Parse("33333333-3333-3333-3333-333333333333"), Name = "İnzibati Məhkəmə" },
                new Court { Id = Guid.Parse("44444444-4444-4444-4444-444444444444"), Name = "Kommersiya Məhkəməsi" },
                new Court { Id = Guid.Parse("55555555-5555-5555-5555-555555555555"), Name = "Ağır Cinayətlər Məhkəməsi" },
                new Court { Id = Guid.Parse("66666666-6666-6666-6666-666666666666"), Name = "Hərbi Məhkəmə" },
                new Court { Id = Guid.Parse("77777777-7777-7777-7777-777777777777"), Name = "Rayon (Şəhər) Məhkəmələri" }
                );
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
                .Where(e => e.Entity is TelimAPI.Domain.Entities.Common.BaseEntity &&
                            (e.State == EntityState.Added));

            foreach (var entry in entries)
            {
                ((TelimAPI.Domain.Entities.Common.BaseEntity)entry.Entity).CreatedDate = DateTime.UtcNow;
            }
        }
    }
}
