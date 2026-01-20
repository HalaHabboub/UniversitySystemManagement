using System.ComponentModel.DataAnnotations;

namespace UniversitySystemManagement.Models
{
    // Department entity - demonstrates 1:M relationships
    // One department has many instructors and many courses
    public class Department
    {
        public int DepartmentId { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        // Navigation property for 1:M (one department has many instructors)
        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();

        // Navigation property for 1:M (one department has many courses)
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}

