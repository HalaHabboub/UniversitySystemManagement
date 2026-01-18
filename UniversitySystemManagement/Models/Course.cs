using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystemManagement.Models
{
    public class Course
    {
        public int CourseId { get; set; }
        [Required, MaxLength(100)]
        public string Title { get; set; }
        [Required, Range(1, 6, ErrorMessage = "Credits must be between 1 and 6 ")]
        public int Credits { get; set; } // VALIDATION RULE
        public int DepartmentId { get; set; }
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        public int? InstructorId { get; set; }
        [ForeignKey("InstructorId")]
        public Instructor? Instructor { get; set; }
        public ICollection<Enrollment> Enrollments { get; set; }
}
}
