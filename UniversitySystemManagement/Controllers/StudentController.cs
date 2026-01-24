using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystemManagement.Data;
using UniversitySystemManagement.Models;

namespace UniversitySystemManagement.Controllers
{
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get current user's ID from claims
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Get student record for current user (null if not completed profile)
        private async Task<Student?> GetCurrentStudentAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.Students
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Department)
                .Include(s => s.Enrollments)
                    .ThenInclude(e => e.Course)
                        .ThenInclude(c => c.Instructor)
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        // GET: Student/CompleteProfile
        public async Task<IActionResult> CompleteProfile()
        {
            var existing = await _context.Students.FirstOrDefaultAsync(s => s.UserId == GetCurrentUserId());
            if (existing != null)
                return RedirectToAction(nameof(MyCourses));

            return View();
        }

        // POST: Student/CompleteProfile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(string firstName, string lastName)
        {
            var userId = GetCurrentUserId();

            var existing = await _context.Students.FirstOrDefaultAsync(s => s.UserId == userId);
            if (existing != null)
                return RedirectToAction(nameof(MyCourses));

            var student = new Student
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = User.Identity?.Name ?? "",
                EnrollmentDate = DateTime.Now
            };

            _context.Students.Add(student);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCourses));
        }

        // GET: Student/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            ViewBag.Student = student;
            ViewBag.GPA = CalculateGPA(student.Enrollments);

            return View(student.Enrollments.ToList());
        }

        // GET: Student/MyGPA
        public async Task<IActionResult> MyGPA()
        {
            var student = await GetCurrentStudentAsync();
            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            var enrollments = student.Enrollments.Where(e => e.Mark.HasValue).ToList();

            ViewBag.Student = student;
            ViewBag.GPA = CalculateGPA(student.Enrollments);
            ViewBag.TotalCredits = enrollments.Sum(e => e.Course?.Credits ?? 0);
            ViewBag.CompletedCourses = enrollments.Count;

            return View(enrollments);
        }

        // Calculate GPA from enrollments
        private decimal CalculateGPA(ICollection<Enrollment> enrollments)
        {
            var graded = enrollments.Where(e => e.Mark.HasValue && e.Course != null).ToList();
            if (!graded.Any()) return 0;

            decimal totalPoints = 0;
            int totalCredits = 0;

            foreach (var e in graded)
            {
                var credits = e.Course!.Credits;
                var gradePoint = ConvertToGradePoint(e.Mark!.Value);
                totalPoints += gradePoint * credits;
                totalCredits += credits;
            }

            return totalCredits > 0 ? Math.Round(totalPoints / totalCredits, 2) : 0;
        }

        // Convert mark (0-100) to grade point (0-4.0)
        private decimal ConvertToGradePoint(decimal mark)
        {
            return mark switch
            {
                >= 90 => 4.0m,
                >= 85 => 3.7m,
                >= 80 => 3.3m,
                >= 75 => 3.0m,
                >= 70 => 2.7m,
                >= 65 => 2.3m,
                >= 60 => 2.0m,
                >= 55 => 1.7m,
                >= 50 => 1.0m,
                _ => 0.0m
            };
        }

        // GET: Student/MyCard
        public async Task<IActionResult> MyCard()
        {
            var student = await _context.Students
                .Include(s => s.StudentCard)
                .FirstOrDefaultAsync(s => s.UserId == GetCurrentUserId());

            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            // If no card exists, redirect to create
            if (student.StudentCard == null)
                return RedirectToAction(nameof(CreateCard));

            ViewBag.Student = student;
            return View(student.StudentCard);
        }

        // GET: Student/CreateCard
        public async Task<IActionResult> CreateCard()
        {
            var student = await _context.Students
                .Include(s => s.StudentCard)
                .FirstOrDefaultAsync(s => s.UserId == GetCurrentUserId());

            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            // If card already exists, redirect to view it
            if (student.StudentCard != null)
                return RedirectToAction(nameof(MyCard));

            ViewBag.Student = student;
            return View();
        }

        // POST: Student/CreateCard
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCard(bool confirm)
        {
            var student = await _context.Students
                .Include(s => s.StudentCard)
                .FirstOrDefaultAsync(s => s.UserId == GetCurrentUserId());

            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            // If card already exists, redirect to view it
            if (student.StudentCard != null)
                return RedirectToAction(nameof(MyCard));

            // Auto-generate card details
            var issueDate = DateTime.Now;
            var cardNumber = $"STU-{issueDate:yyyyMMdd}-{student.Id:D4}";

            var studentCard = new StudentCard
            {
                StudentId = student.Id,
                CardNumber = cardNumber,
                IssueDate = issueDate,
                ExpiryDate = issueDate.AddYears(1),
                IsActive = true
            };

            _context.StudentCards.Add(studentCard);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCard));
        }

        // GET: Student/EditCard
        public async Task<IActionResult> EditCard()
        {
            var student = await _context.Students
                .Include(s => s.StudentCard)
                .FirstOrDefaultAsync(s => s.UserId == GetCurrentUserId());

            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            // If no card exists, redirect to create
            if (student.StudentCard == null)
                return RedirectToAction(nameof(CreateCard));

            ViewBag.Student = student;
            return View(student.StudentCard);
        }

        // POST: Student/EditCard
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditCard(bool isActive)
        {
            var student = await _context.Students
                .Include(s => s.StudentCard)
                .FirstOrDefaultAsync(s => s.UserId == GetCurrentUserId());

            if (student == null)
                return RedirectToAction(nameof(CompleteProfile));

            if (student.StudentCard == null)
                return RedirectToAction(nameof(CreateCard));

            student.StudentCard.IsActive = isActive;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(MyCard));
        }
    }
}
