// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace QuanLyNhatro.Areas.Identity.Pages.Account
{
    public class ResetPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;

        public ResetPasswordModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public InputModel Input { get; set; }

        public class InputModel
        {
            [Required(ErrorMessage = "Email là bắt buộc")]
            [EmailAddress(ErrorMessage = "Email không hợp lệ")]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
            [StringLength(100, ErrorMessage = "Mật khẩu phải có ít nhất {2} ký tự và tối đa {1} ký tự.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Mật khẩu mới")]
            public string Password { get; set; }

            [DataType(DataType.Password)]
            [Display(Name = "Xác nhận mật khẩu")]
            [Compare("Password", ErrorMessage = "Mật khẩu và xác nhận mật khẩu không khớp.")]
            public string ConfirmPassword { get; set; }

            [Required]
            public string Code { get; set; }
        }

        public IActionResult OnGet(string code = null)
        {
            if (code == null)
            {
                return BadRequest("A code must be supplied for password reset.");
            }
            else
            {
                Input = new InputModel
                {
                    Code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code))
                };
                return Page();
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            var result = await _userManager.ResetPasswordAsync(user, Input.Code, Input.Password);
            if (result.Succeeded)
            {
                return RedirectToPage("./ResetPasswordConfirmation");
            }

            // Dịch error messages sang tiếng Việt
            foreach (var error in result.Errors)
            {
                string errorMessage = error.Description;

                // Dịch các error phổ biến
                if (error.Code == "PasswordRequiresNonAlphanumeric")
                    errorMessage = "Mật khẩu phải chứa ít nhất một ký tự đặc biệt (!@#$%^&*).";
                else if (error.Code == "PasswordRequiresLower")
                    errorMessage = "Mật khẩu phải có ít nhất một chữ cái viết thường ('a'-'z').";
                else if (error.Code == "PasswordRequiresUpper")
                    errorMessage = "Mật khẩu phải có ít nhất một chữ cái viết hoa ('A'-'Z').";
                else if (error.Code == "PasswordRequiresDigit")
                    errorMessage = "Mật khẩu phải có ít nhất một chữ số ('0'-'9').";
                else if (error.Code == "PasswordTooShort")
                    errorMessage = "Mật khẩu phải có ít nhất 6 ký tự.";
                else if (error.Code == "InvalidToken")
                    errorMessage = "Link đặt lại mật khẩu không hợp lệ hoặc đã hết hạn.";

                ModelState.AddModelError(string.Empty, errorMessage);
            }

            return Page();
        }
    }
}