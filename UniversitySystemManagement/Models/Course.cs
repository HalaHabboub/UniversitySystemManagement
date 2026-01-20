using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystemManagement.Models
{
    // Course entity - central to the system
    // Has M:M with Student (via Enrollment), and 1:M with Department and Instructor
    public class Course
    {
        public int CourseId { get; set; }

        [Required, MaxLength(100)]
        public string Title { get; set; } = string.Empty;

        // Optional course description
        [MaxLength(500)]
        public string? Description { get; set; }

        // Validation: credits must be between 1 and 6
        [Required, Range(1, 6, ErrorMessage = "Credits must be between 1 and 6")]
        public int Credits { get; set; }

        // Foreign Key for Department (1:M - course belongs to one department)
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Foreign Key for Instructor (1:M - course taught by one instructor, nullable)
        public int? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public Instructor? Instructor { get; set; }

        // Navigation property for M:M relationship with Student
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();
    }
}
