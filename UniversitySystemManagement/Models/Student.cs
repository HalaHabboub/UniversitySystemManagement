using System.Collections;

namespace UniversitySystemManagement.Models
{
    public class Student : Person
    {
        public DateTime EnrollmentDate { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; } // Many-to-Many
}
}
