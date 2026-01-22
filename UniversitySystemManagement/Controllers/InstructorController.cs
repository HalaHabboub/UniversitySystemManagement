using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UniversitySystemManagement.Data;
using UniversitySystemManagement.Models;

namespace UniversitySystemManagement.Controllers
{
    [Authorize(Roles = "Instructor")]
    public class InstructorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public InstructorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Get current user's ID from claims
        private string? GetCurrentUserId()
        {
            return User.FindFirstValue(ClaimTypes.NameIdentifier);
        }

        // Get instructor record for current user (null if not completed profile)
        private async Task<Instructor?> GetCurrentInstructorAsync()
        {
            var userId = GetCurrentUserId();
            return await _context.Instructors
                .Include(i => i.Department)
                .FirstOrDefaultAsync(i => i.UserId == userId);
        }

        // Check if profile exists, redirect to CompleteProfile if not
        private async Task<Instructor?> RequireProfileAsync()
        {
            var instructor = await GetCurrentInstructorAsync();
            return instructor;
        }

        // GET: Instructor/CompleteProfile - Show form to complete profile
        public async Task<IActionResult> CompleteProfile()
        {
            // If already has profile, redirect to dashboard
            var existing = await GetCurrentInstructorAsync();
            if (existing != null)
                return RedirectToAction(nameof(Dashboard));

            ViewBag.Departments = new SelectList(_context.Departments, "DepartmentId", "Name");
            return View();
        }

        // POST: Instructor/CompleteProfile - Save profile
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CompleteProfile(string firstName, string lastName, int departmentId)
        {
            var userId = GetCurrentUserId();

            // Check if already exists
            var existing = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == userId);
            if (existing != null)
                return RedirectToAction(nameof(Dashboard));

            var instructor = new Instructor
            {
                UserId = userId,
                FirstName = firstName,
                LastName = lastName,
                Email = User.Identity?.Name ?? "",
                HireDate = DateTime.Now,
                DepartmentId = departmentId
            };

            _context.Instructors.Add(instructor);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Dashboard));
        }

        // GET: Instructor/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructor.Id)
                .Include(c => c.Department)
                .Include(c => c.Enrollments)
                .ToListAsync();

            ViewBag.Instructor = instructor;
            ViewBag.CourseCount = courses.Count;
            ViewBag.TotalStudents = courses.Sum(c => c.Enrollments.Count);

            return View(courses);
        }

        // GET: Instructor/MyCourses
        public async Task<IActionResult> MyCourses()
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            var courses = await _context.Courses
                .Where(c => c.InstructorId == instructor.Id)
                .Include(c => c.Department)
                .Include(c => c.Enrollments)
                .ToListAsync();

            return View(courses);
        }

        // GET: Instructor/CourseDetails/5
        public async Task<IActionResult> CourseDetails(int id)
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            var course = await _context.Courses
                .Include(c => c.Department)
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id && c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // GET: Instructor/CourseStudents/5
        public async Task<IActionResult> CourseStudents(int id)
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            var course = await _context.Courses
                .Include(c => c.Enrollments)
                    .ThenInclude(e => e.Student)
                .FirstOrDefaultAsync(c => c.CourseId == id && c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            return View(course);
        }

        // POST: Instructor/UpdateMark
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateMark(int courseId, int studentId, decimal? mark)
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            // Verify course belongs to instructor
            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            var enrollment = await _context.Enrollments
                .FirstOrDefaultAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (enrollment == null)
                return NotFound();

            enrollment.Mark = mark;
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(CourseStudents), new { id = courseId });
        }

        // GET: Instructor/EnrollStudent/5
        public async Task<IActionResult> EnrollStudent(int id)
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == id && c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            // Get students not already enrolled
            var enrolledStudentIds = await _context.Enrollments
                .Where(e => e.CourseId == id)
                .Select(e => e.StudentId)
                .ToListAsync();

            var availableStudents = await _context.Students
                .Where(s => !enrolledStudentIds.Contains(s.Id))
                .ToListAsync();

            ViewBag.Course = course;
            ViewBag.Students = new SelectList(availableStudents, "Id", "Email");

            return View();
        }

        // POST: Instructor/EnrollStudent
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EnrollStudent(int courseId, int studentId)
        {
            var instructor = await RequireProfileAsync();
            if (instructor == null)
                return RedirectToAction(nameof(CompleteProfile));

            var course = await _context.Courses
                .FirstOrDefaultAsync(c => c.CourseId == courseId && c.InstructorId == instructor.Id);

            if (course == null)
                return NotFound();

            var exists = await _context.Enrollments
                .AnyAsync(e => e.CourseId == courseId && e.StudentId == studentId);

            if (!exists)
            {
                _context.Enrollments.Add(new Enrollment
                {
                    CourseId = courseId,
                    StudentId = studentId,
                    Mark = null
                });
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(CourseStudents), new { id = courseId });
        }
    }
}
