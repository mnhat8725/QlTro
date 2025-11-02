using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using QuanLyNhaTro.Data;
using QuanLyNhaTro.Models;

namespace QuanLyNhaTro.Controllers
{
    [Authorize(Roles = "Admin")]
    public class UserManagementController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UserManagementController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // Danh sách người thuê chưa có tài khoản
        public async Task<IActionResult> Index()
        {
            // Lấy tất cả người thuê
            var allNguoiThues = await _context.NguoiThues.ToListAsync();

            // Lấy danh sách username đã tồn tại
            var existingUserNames = await _userManager.Users
                .Select(u => u.UserName)
                .ToListAsync();

            // Filter người thuê chưa có tài khoản
            var nguoiThuesChuaCoTaiKhoan = allNguoiThues
                .Where(nt => !existingUserNames.Contains(nt.SoDienThoai))
                .ToList();

            return View(nguoiThuesChuaCoTaiKhoan);
        }

        // Tạo tài khoản cho người thuê
        [HttpPost]
        public async Task<IActionResult> CreateAccount(int nguoiThueId)
        {
            try
            {
                // Lấy thông tin người thuê
                var nguoiThue = await _context.NguoiThues.FindAsync(nguoiThueId);
                if (nguoiThue == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy người thuê!";
                    return RedirectToAction(nameof(Index));
                }

                // USERNAME = SỐ ĐIỆN THOẠI
                var userName = nguoiThue.SoDienThoai;

                // EMAIL = SĐT@nhatro.com
                var email = $"{userName}@nhatro.com";

                // Kiểm tra username đã tồn tại chưa
                var existingUserByUsername = await _userManager.FindByNameAsync(userName);
                if (existingUserByUsername != null)
                {
                    TempData["WarningMessage"] = $"Số điện thoại {userName} đã được sử dụng cho tài khoản khác!";
                    return RedirectToAction(nameof(Index));
                }

                // Kiểm tra email đã tồn tại chưa
                var existingUserByEmail = await _userManager.FindByEmailAsync(email);
                if (existingUserByEmail != null)
                {
                    // Nếu email trùng, thêm số random
                    email = $"{userName}.{DateTime.Now.Ticks % 10000}@nhatro.com";
                }

                // Tạo tài khoản mới
                var user = new IdentityUser
                {
                    UserName = userName,
                    Email = email,
                    EmailConfirmed = true,
                    PhoneNumber = userName,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    TwoFactorEnabled = false
                };

                // Mật khẩu: NhaTro@SĐT
                var password = $"NhaTro@{userName}";
                var result = await _userManager.CreateAsync(user, password);

                if (result.Succeeded)
                {
                    // Đảm bảo role NguoiThue tồn tại
                    if (!await _roleManager.RoleExistsAsync("NguoiThue"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("NguoiThue"));
                    }

                    // Gán role
                    await _userManager.AddToRoleAsync(user, "NguoiThue");

                    TempData["SuccessMessage"] = "Tạo tài khoản thành công!";
                    TempData["AccountInfo"] = $"<div class='mb-2'><strong>Tài khoản cho {nguoiThue.HoTen}:</strong></div>" +
                                              $"<div class='mb-1'>📱 <strong>Tài khoản:</strong> {userName}</div>" +
                                              $"<div class='mb-1'>📧 <strong>Email:</strong> {email}</div>" +
                                              $"<div class='mb-1'>🔑 <strong>Mật khẩu:</strong> {password}</div>" +
                                              $"<div class='mt-2 alert alert-warning p-2'>" +
                                              $"<strong>⚠️ QUAN TRỌNG:</strong><br/>" +
                                              $"Người thuê đăng nhập bằng <strong>SỐ ĐIỆN THOẠI</strong>: <code>{userName}</code><br/>" +
                                              $"<small>(Không cần nhập email)</small>" +
                                              $"</div>";
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Không tạo được tài khoản: {errors}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Tạo tài khoản hàng loạt
        [HttpPost]
        public async Task<IActionResult> CreateAllAccounts()
        {
            try
            {
                var allNguoiThues = await _context.NguoiThues.ToListAsync();
                var existingUserNames = await _userManager.Users
                    .Select(u => u.UserName)
                    .ToListAsync();

                var nguoiThuesChuaCoTaiKhoan = allNguoiThues
                    .Where(nt => !existingUserNames.Contains(nt.SoDienThoai))
                    .ToList();

                int successCount = 0;
                int failCount = 0;
                var failedUsers = new List<string>();

                // Đảm bảo role tồn tại
                if (!await _roleManager.RoleExistsAsync("NguoiThue"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("NguoiThue"));
                }

                foreach (var nguoiThue in nguoiThuesChuaCoTaiKhoan)
                {
                    var userName = nguoiThue.SoDienThoai;
                    var email = $"{userName}@nhatro.com";

                    // Kiểm tra email trùng
                    var existingEmail = await _userManager.FindByEmailAsync(email);
                    if (existingEmail != null)
                    {
                        // Tạo email unique
                        email = $"{userName}.{DateTime.Now.Ticks % 10000}@nhatro.com";
                    }

                    var user = new IdentityUser
                    {
                        UserName = userName,
                        Email = email,
                        EmailConfirmed = true,
                        PhoneNumber = userName,
                        PhoneNumberConfirmed = true,
                        LockoutEnabled = false,
                        TwoFactorEnabled = false
                    };

                    var password = $"NhaTro@{userName}";
                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await _userManager.AddToRoleAsync(user, "NguoiThue");
                        successCount++;
                    }
                    else
                    {
                        failCount++;
                        var errorMsg = result.Errors.FirstOrDefault()?.Description ?? "Unknown error";
                        failedUsers.Add($"{nguoiThue.HoTen} ({errorMsg})");
                    }
                }

                TempData["SuccessMessage"] = $"Đã tạo {successCount} tài khoản thành công!";
                if (failCount > 0)
                {
                    TempData["WarningMessage"] = $"Có {failCount} tài khoản tạo thất bại: {string.Join(", ", failedUsers.Take(5))}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // Reset password
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string userName)
        {
            try
            {
                var user = await _userManager.FindByNameAsync(userName);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Không tìm thấy user!";
                    return RedirectToAction(nameof(Index));
                }

                var removeResult = await _userManager.RemovePasswordAsync(user);
                if (!removeResult.Succeeded)
                {
                    TempData["ErrorMessage"] = "Không thể xóa mật khẩu cũ!";
                    return RedirectToAction(nameof(Index));
                }

                var newPassword = $"NhaTro@{userName}";
                var addResult = await _userManager.AddPasswordAsync(user, newPassword);

                if (addResult.Succeeded)
                {
                    // Unlock user
                    user.LockoutEnabled = false;
                    user.LockoutEnd = null;
                    user.AccessFailedCount = 0;
                    await _userManager.UpdateAsync(user);

                    TempData["SuccessMessage"] = $"Reset mật khẩu thành công!";
                    TempData["AccountInfo"] = $"<div class='mb-2'><strong>Thông tin đăng nhập:</strong></div>" +
                                              $"<div class='mb-1'>📱 <strong>Tài khoản:</strong> {userName}</div>" +
                                              $"<div class='mb-1'>🔑 <strong>Mật khẩu mới:</strong> {newPassword}</div>";
                }
                else
                {
                    var errors = string.Join(", ", addResult.Errors.Select(e => e.Description));
                    TempData["ErrorMessage"] = $"Lỗi: {errors}";
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // XÓA TẤT CẢ USER TEST (Debug only)
        [HttpGet]
        public async Task<IActionResult> DeleteAllTestUsers()
        {
            try
            {
                var testUsers = await _userManager.Users
                    .Where(u => u.Email.Contains("@nhatro.com") || u.Email.Contains("@test.com"))
                    .ToListAsync();

                int deleteCount = 0;
                foreach (var user in testUsers)
                {
                    var result = await _userManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        deleteCount++;
                    }
                }

                TempData["SuccessMessage"] = $"Đã xóa {deleteCount} tài khoản test!";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Lỗi: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}