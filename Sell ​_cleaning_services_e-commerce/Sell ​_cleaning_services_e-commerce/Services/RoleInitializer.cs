using Microsoft.AspNetCore.Identity;
using Sell__cleaning_services_e_commerce.Models;



namespace Sell_​_cleaning_services_e_commerce.Services
{
  

 
        public class RoleInitializer
        {

        public static async Task InitializeAsync(RoleManager<Role> roleManager)
        {
            string[] roleNames = { "Admin", "Customer" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new Role { Name = roleName });
                }
            }
        }


        public static async Task AssignPermissionsToUsers(IServiceProvider serviceProvider, string email, string role)
            {
                var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

                // Tìm người dùng dựa trên email
                var user = await userManager.FindByEmailAsync(email);
                if (user != null)
                {
                    // Kiểm tra xem người dùng đã có vai trò hay chưa
                    var roles = await userManager.GetRolesAsync(user);
                    if (!roles.Contains(role))
                    {
                        // Gán vai trò cho người dùng nếu chưa có
                        await userManager.AddToRoleAsync(user, role);
                    }
                }
            }


        }
    }
