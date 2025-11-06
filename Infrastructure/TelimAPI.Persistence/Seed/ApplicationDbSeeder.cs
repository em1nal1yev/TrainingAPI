using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TelimAPI.Domain.Entities;

namespace TelimAPI.Persistence.Seed
{
    public static class ApplicationDbSeeder
    {
        public static async Task SeedAsync(UserManager<User> userManager, RoleManager<IdentityRole<Guid>> roleManager)
        {
            string[] roles = { "Admin", "Trainer", "User" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<Guid>(role));
                }
            }

            var adminEmail = "eminal1y3v@gmail.com";
            if (await userManager.FindByEmailAsync(adminEmail) == null)
            {
                var defaultCourtId = Guid.Parse("11111111-1111-1111-1111-111111111111");
                var defaultDepartmentId = Guid.Parse("00000000-0000-0000-0000-000000000001");
                var admin = new User
                {
                    UserName = "admin",
                    Email = adminEmail,
                    Name = "System",
                    Surname = "Admin",
                    EmailConfirmed = true,
                    CourtId = defaultCourtId,
                    DepartmentId = defaultDepartmentId
                };

                var result = await userManager.CreateAsync(admin, "Admin123!");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
            }
        }
    }
}
