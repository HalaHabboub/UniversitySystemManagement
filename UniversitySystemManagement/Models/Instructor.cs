using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystemManagement.Models
{
    // Instructor inherits from Person (demonstrates inheritance)
    // Has 1:M relationship with Course (one instructor teaches many courses)
    public class Instructor : Person
    {
        public DateTime HireDate { get; set; }

        // Foreign Key for Department (1:M - instructor belongs to one department)
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        // Navigation property for 1:M relationship (one instructor teaches many courses)
        public ICollection<Course> Courses { get; set; } = new List<Course>();
    }
}
