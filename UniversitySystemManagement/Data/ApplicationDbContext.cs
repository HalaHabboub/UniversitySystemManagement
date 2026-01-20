using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystemManagement.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : IdentityDbContext(options)
    {
        // Define DbSets for each entity
        public DbSet<UniversitySystemManagement.Models.Department> Departments { get; set; }
        public DbSet<UniversitySystemManagement.Models.Instructor> Instructors { get; set; }
        public DbSet<UniversitySystemManagement.Models.Student> Students { get; set; }
        public DbSet<UniversitySystemManagement.Models.Course> Courses { get; set; }
        public DbSet<UniversitySystemManagement.Models.Enrollment> Enrollments { get; set; }
        public DbSet<UniversitySystemManagement.Models.StudentCard> StudentCards { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        { 
            // IMPORTANT: Must call base for Identity tables because I am overriding OnModelCreating
            base.OnModelCreating(modelBuilder);        
            
            // Configure composite key for Enrollment (M:M bridge table)
            modelBuilder.Entity<UniversitySystemManagement.Models.Enrollment>()
                .HasKey(e => new { e.StudentId, e.CourseId });

            // Configure foreign keys for Enrollment
            modelBuilder.Entity<UniversitySystemManagement.Models.Enrollment>()
                .HasOne(e => e.Student)
                .WithMany(s => s.Enrollments)
                .HasForeignKey(e => e.StudentId);
            modelBuilder.Entity<UniversitySystemManagement.Models.Enrollment>()
                .HasOne(e => e.Course)
                .WithMany(c => c.Enrollments)
                .HasForeignKey(e => e.CourseId);

            // Configure 1:1 relationship between Student and StudentCard
            // StudentCard uses Student's primary key as both PK and FK (shared primary key)
            modelBuilder.Entity<UniversitySystemManagement.Models.StudentCard>()
                .HasOne(sc => sc.Student)
                .WithOne(s => s.StudentCard)
                .HasForeignKey<UniversitySystemManagement.Models.StudentCard>(sc => sc.StudentId);

        }
    }
}
