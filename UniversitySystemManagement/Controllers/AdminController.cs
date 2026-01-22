using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UniversitySystemManagement.Data;

namespace UniversitySystemManagement.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _context;

        public AdminController(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _context = context;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            ViewBag.UserCount = await _userManager.Users.CountAsync();
            ViewBag.RoleCount = await _roleManager.Roles.CountAsync();
            ViewBag.StudentCount = await _context.Students.CountAsync();
            ViewBag.InstructorCount = await _context.Instructors.CountAsync();
            ViewBag.CourseCount = await _context.Courses.CountAsync();
            return View();
        }

        // GET: Admin/Users
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new List<(IdentityUser User, IList<string> Roles)>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add((user, roles));
            }

            return View(userRoles);
        }

        // GET: Admin/UserDetails/userId
        public async Task<IActionResult> UserDetails(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.Roles = await _userManager.GetRolesAsync(user);
            ViewBag.Student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == id);
            ViewBag.Instructor = await _context.Instructors
                .Include(i => i.Department)
                .FirstOrDefaultAsync(i => i.UserId == id);

            return View(user);
        }

        // GET: Admin/SetRole/userId
        public async Task<IActionResult> SetRole(string id)
        {
            if (string.IsNullOrEmpty(id))
                return NotFound();

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            ViewBag.UserId = user.Id;
            ViewBag.UserEmail = user.Email;
            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);
            // Only show Student and Instructor roles (Admin role is not assignable via UI)
            ViewBag.AllRoles = new List<string> { "Student", "Instructor" };

            return View();
        }

        // POST: Admin/SetRole - Only assigns role, user completes profile on first login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Remove current roles
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign new role (profile will be created when user logs in)
            if (!string.IsNullOrEmpty(role))
            {
                await _userManager.AddToRoleAsync(user, role);
            }

            return RedirectToAction(nameof(Users));
        }

        // POST: Admin/DeleteUser
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUser(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
                return NotFound();

            if (user.Email == User.Identity?.Name)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }

            // Delete linked Student/Instructor records
            var student = await _context.Students.FirstOrDefaultAsync(s => s.UserId == id);
            if (student != null) _context.Students.Remove(student);

            var instructor = await _context.Instructors.FirstOrDefaultAsync(i => i.UserId == id);
            if (instructor != null) _context.Instructors.Remove(instructor);

            await _context.SaveChangesAsync();
            await _userManager.DeleteAsync(user);

            return RedirectToAction(nameof(Users));
        }
    }
}
