using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace UniversitySystemManagement.Models
{
    // StudentCard entity - demonstrates 1:1 relationship with Student
    // Each student has exactly one card, and each card belongs to exactly one student
    public class StudentCard
    {
        // Primary key - also serves as foreign key to Student (shared primary key pattern)
        // This ensures true 1:1 relationship at database level
        [Key]
        [ForeignKey("Student")]
        public int StudentId { get; set; }

        // Unique card number issued to the student
        [Required, MaxLength(20)]
        public string CardNumber { get; set; } = string.Empty;

        // Date when the card was issued
        public DateTime IssueDate { get; set; }

        // Date when the card expires
        public DateTime ExpiryDate { get; set; }

        // Whether the card is currently active
        public bool IsActive { get; set; } = true;

        // Navigation property for 1:1 relationship back to Student
        public Student? Student { get; set; }
    }
}
