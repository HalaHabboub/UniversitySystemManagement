using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystemManagement.Models
{
    public class Instructor : Person
    {
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; } // Foreign Key
        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; } // Navigation
        public ICollection<Course> Courses { get; set; }
}
}
