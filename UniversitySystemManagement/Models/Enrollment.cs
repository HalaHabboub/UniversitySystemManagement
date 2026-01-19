using System.ComponentModel.DataAnnotations;

namespace UniversitySystemManagement.Models
{
    // Bridge/Join table for M:M relationship between Student and Course
    // Composite primary key (StudentId + CourseId) configured in DbContext
    // Contains extra property (Mark) - this is why I use explicit join entity instead of skip navigation
    public class Enrollment
    {
        // Part of composite primary key
        public int StudentId { get; set; }

        // Part of composite primary key
        public int CourseId { get; set; }

        // Extra property in the relationship - student's mark for this course
        [Range(0, 100)]
        public decimal? Mark { get; set; }

        // Navigation properties
        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}
