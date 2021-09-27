using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CustomerManagement.Data
{
    public class SeedData
    {
        private readonly AppDbContext dbContext;

        public SeedData(AppDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public static void SeedDatas(
            RoleManager<IdentityRole> roleManager, UserManager<ApplicationUser> userManager
            )
        {
            SeedRoles(roleManager);
            SeedUsers(userManager);
        }
        public static void SeedRoles(RoleManager<IdentityRole> roleManager)
        {
            if (!roleManager.RoleExistsAsync("admin").Result)
            {
                IdentityRole adminRole = new IdentityRole();

                adminRole.Name = "admin";
                adminRole.NormalizedName = "ADMIN";
                IdentityResult adminRoleResult = roleManager.CreateAsync(adminRole).Result;
            }
            if (!roleManager.RoleExistsAsync("customer").Result)
            {
                IdentityRole customerRole = new IdentityRole();

                customerRole.Name = "customer";
                customerRole.NormalizedName = "CUSTOMER";
                IdentityResult customerRoleResult = roleManager.CreateAsync(customerRole).Result;
            }
        }

        public static void SeedUsers(UserManager<ApplicationUser> userManager)
        {

            if (userManager.FindByNameAsync("admin@admin.com").Result == null)
            {

                ApplicationUser adminUser = new ApplicationUser();
                adminUser.UserName = "admin@admin.com";
                adminUser.Email = "admin@admin.com";
                adminUser.FirstName = "Admin";
                adminUser.LastName = "Admin";
                adminUser.EmailConfirmed = true;

                try
                {
                    IdentityResult adminUserResult = userManager.CreateAsync(adminUser, "Password123$").Result;
                    if (adminUserResult.Succeeded)
                    {

                        userManager.AddToRoleAsync(adminUser, "admin").Wait();
                    }

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

            }
        }
    }
}
