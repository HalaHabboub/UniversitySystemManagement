using Microsoft.AspNetCore.Identity;

namespace UniversitySystemManagement.Data
{
    /// Seeds default roles and admin user into the database on application startup.

    public static class RoleSeeder
    {
        /// IServiceProvider: The dependency injection container that holds all registered services.
        /// It allows us to request (resolve) services like RoleManager and UserManager at runtime.
        
        public static async Task SeedRolesAndAdminAsync(IServiceProvider serviceProvider)
        {
            // RoleManager<IdentityRole>: A service provided by ASP.NET Identity to manage roles.
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // UserManager<IdentityUser>: A service provided by ASP.NET Identity to manage users.
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Define the three roles our system needs
            string[] roles = { "Admin", "Instructor", "Student" };

            // Create each role if it doesn't already exist
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed a default admin user so we can access admin features immediately
            string adminEmail = "admin@xyz.edu.jo";
            string adminPassword = "Admin@123";

            // Check if admin user already exists
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true  // Skip email confirmation for seeded admin
                };

                // CreateAsync hashes the password and saves user to AspNetUsers table
                var result = await userManager.CreateAsync(adminUser, adminPassword);

                if (result.Succeeded)
                {
                    // AddToRoleAsync assigns the "Admin" role to this user
                    // This creates a record in AspNetUserRoles linking user to role
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
