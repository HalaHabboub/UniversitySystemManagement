using System.ComponentModel.DataAnnotations;

namespace UniversitySystemManagement.Models
{
    public class Department
    {
        public int DepartmentId { get; set; }
        [Required, MaxLength(100)]
        public string Name { get; set; }
        public ICollection <Instructor> Instructors { get; set; } // One-to-Many
        
        public ICollection<Course> Courses { get; set; } // One-to-Many
}
    }
}
