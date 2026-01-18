using System.ComponentModel.DataAnnotations;

namespace UniversitySystemManagement.Models
{
    public abstract class Person
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string FirstName { get; set; }

        [Required, MaxLength(100)]
        public string LastName { get; set; }

        [Required, EmailAddress]
        public string Email { get; set; }

        public virtual string FullName() => $"{FirstName} {LastName}";
    }
}
