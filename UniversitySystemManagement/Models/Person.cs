using System.ComponentModel.DataAnnotations;

namespace UniversitySystemManagement.Models
{
    // Abstract base class for inheritance - Student and Instructor inherit from this
    public abstract class Person
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required, MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required, EmailAddress]
        public string Email { get; set; } = string.Empty;

        // Virtual method that can be overridden by derived classes
        public virtual string FullName() => $"{FirstName} {LastName}";
    }
}
