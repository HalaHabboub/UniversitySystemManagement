using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace UniversitySystemManagement.Controllers
{
    // Only users with Admin role can access this controller
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // Inject UserManager and RoleManager via constructor
        public AdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: Admin/Dashboard
        public async Task<IActionResult> Dashboard()
        {
            // Pass counts to view for quick stats
            ViewBag.UserCount = await _userManager.Users.CountAsync();
            ViewBag.RoleCount = await _roleManager.Roles.CountAsync();
            return View();
        }

        // GET: Admin/Users - List all users with their roles
        public async Task<IActionResult> Users()
        {
            var users = await _userManager.Users.ToListAsync();

            // Build a list of users with their roles for the view
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

            //we are using ViewBag to pass data to the view
            // User ID and Email for display
            ViewBag.UserId = user.Id;
            ViewBag.UserEmail = user.Email;
            ViewBag.CurrentRoles = await _userManager.GetRolesAsync(user);
            ViewBag.AllRoles = await _roleManager.Roles.Select(r => r.Name).ToListAsync();

            return View();
        }

        // POST: Admin/SetRole
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return NotFound();

            // Remove all current roles first
            var currentRoles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, currentRoles);

            // Assign the new role
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

            // Prevent deleting yourself
            if (user.Email == User.Identity?.Name)
            {
                TempData["Error"] = "You cannot delete your own account.";
                return RedirectToAction(nameof(Users));
            }

            await _userManager.DeleteAsync(user);
            return RedirectToAction(nameof(Users));
        }
    }
}
