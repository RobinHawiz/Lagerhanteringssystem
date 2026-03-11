using Microsoft.AspNetCore.Identity;

namespace MVC.Models;

public class DbAccountSeeder
{
    public static async Task Seed(IServiceProvider services)
    {
        {
            using var scope = services.CreateScope();

            // Seed roles
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roles = { "Admin", "Warehouse" };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            // Seed admin user
            var email = "admin@company.com";
            var password = "Passwordadmin1234!";

            var userExist = await userManager.FindByEmailAsync(email);
            if(userExist == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
                else
                {
                    throw new Exception("Failed to create the admin user: " + string.Join(", ", result.Errors));
                }
            }

            // Seed warehouse user
            email = "warehouse@company.com";
            password = "Passwordwarehouse1234!";

            userExist = await userManager.FindByEmailAsync(email);
            if (userExist == null)
            {
                var warehouseUser = new IdentityUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(warehouseUser, password);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(warehouseUser, "Warehouse");
                }
                else
                {
                    throw new Exception("Failed to create the warehouse user: " + string.Join(", ", result.Errors));
                }
            }
        }
    }
}
