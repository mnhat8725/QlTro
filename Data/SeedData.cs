using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
namespace QuanLyNhatro.Data
{
    public static class SeedData 
    {
        public static async Task InitializeAsync(IServiceProvider serviceProvider)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = serviceProvider.GetRequiredService<UserManager<IdentityUser>>();

            string[] roleNames = { "Admin", "ChuTro", "NguoiThue" };
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }
            var adminEmail = "admin@nhatro.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var newAdmin = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var createAdmin = await userManager.CreateAsync(newAdmin, "Admin@123");
                if (createAdmin.Succeeded)
                {
                    await userManager.AddToRoleAsync(newAdmin, "Admin");
                }
            }
            // tao tk chu tro
            var chuTroEmail = "chutro@nhatro.com";
            var chuTroUser = await userManager.FindByEmailAsync(chuTroEmail);
            if (chuTroUser == null)
            {
                var newChuTro = new IdentityUser
                {
                    UserName = chuTroEmail,
                    Email = chuTroEmail,
                    EmailConfirmed = true
                };
                var createChuTro = await userManager.CreateAsync(newChuTro, "ChuTro@123");
                if (createChuTro.Succeeded)
                {
                    await userManager.AddToRoleAsync(newChuTro, "ChuTro");
                }
            }
            // tk nguoi thue
            var nguoiThueEmail = "nguoithue@nhatro.com";
            var nguoiThueUser = await userManager.FindByEmailAsync(nguoiThueEmail);
            if (nguoiThueUser == null)
            {
                var newNguoiThue = new IdentityUser
                {
                    UserName = nguoiThueEmail,
                    Email = nguoiThueEmail,
                    EmailConfirmed = true
                };
                var createNguoiThue = await userManager.CreateAsync(newNguoiThue, "NguoiThue@123");
                if (createNguoiThue.Succeeded)
                {
                    await userManager.AddToRoleAsync(newNguoiThue, "NguoiThue");
                }
            }
        }     
    }
}
