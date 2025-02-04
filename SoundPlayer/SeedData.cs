using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using SoundPlayer.DAL;
using SoundPlayer.Domain.Entities;
using Role = SoundPlayer.Domain.Constants.Role;

namespace SoundPlayer
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            // Применяем миграции, если они не были применены
            await context.Database.MigrateAsync();

            await SeedRoles(roleManager);
            await SeedAdminUser(userManager);
        }

        private static async Task SeedRoles(RoleManager<IdentityRole<int>> roleManager)
        {
            var roles = Role.GetRoles();

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role));
                }
            }
        }

        private static async Task SeedAdminUser(UserManager<ApplicationUser> userManager)
        {
            var adminEmail = "admin@email.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                var user = new ApplicationUser
                {
                    UserName = "admin",
                    Email = adminEmail,
                    CreatedTime = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(user, "Admin1.");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, Role.Admin);
                }
            }
        }
    }
}
