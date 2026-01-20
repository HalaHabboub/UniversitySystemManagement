using System.Collections;

namespace UniversitySystemManagement.Models
{
    // Student inherits from Person (demonstrates inheritance)
    // Has M:M relationship with Course through Enrollment bridge table
    // Has 1:1 relationship with StudentCard
    public class Student : Person
    {
        public DateTime EnrollmentDate { get; set; }

        // Navigation property for M:M relationship with Course
        public ICollection<Enrollment> Enrollments { get; set; } = new List<Enrollment>();

        // Navigation property for 1:1 relationship with StudentCard
        public StudentCard? StudentCard { get; set; }
    }
}
