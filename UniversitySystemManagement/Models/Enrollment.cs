using System.ComponentModel.DataAnnotations;

namespace UniversitySystemManagement.Models
{
    public class Enrollment //Joint table for Many-to-Many relationship between Student and Course
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        [Range(0, 100)]
        public decimal? Mark { get; set; } // Numeric marks
        public Student? Student { get; set; }
        public Course? Course { get; set; }
    }
}
