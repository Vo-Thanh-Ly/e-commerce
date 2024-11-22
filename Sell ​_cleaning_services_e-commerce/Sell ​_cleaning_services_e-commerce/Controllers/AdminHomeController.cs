using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Sell__cleaning_services_e_commerce.Areas.MailService;
using Microsoft.AspNetCore.Mvc;
using Sell__cleaning_services_e_commerce.Areas.Other;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Models;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{

    [Authorize(Roles = RoleList.Admin)]
    public class AdminHomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IEmailSender _emailSender;
        private readonly ApplicationDbContext _context;
        public AdminHomeController(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IEmailSender emailSender,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _emailSender = emailSender;
            _context = context;
        }

        public async Task<IActionResult> UserList()
        {
            var adminUsers = await _userManager.GetUsersInRoleAsync(RoleList.Admin);
            var shipperUsers = await _userManager.GetUsersInRoleAsync(RoleList.Shipper);
            var salesperson = await _userManager.GetUsersInRoleAsync(RoleList.Salesperson);
            var filteredUsers = adminUsers.Union(shipperUsers).Union(salesperson)
                .Where(u => u.Email != "vothanhly632002@gmail.com")
                .ToList();

            return View(filteredUsers);
        }

        public async Task<IActionResult> DeleteUser(string id)
        {
            if (string.IsNullOrEmpty(id))
            {
                return BadRequest("Mã người dùng không hợp lệ");
            }

            var user = await _userManager.FindByIdAsync(id);
            if (user == null)
            {
                return NotFound($"Không tìm thấy người dùng với ID: {id}");
            }

            // Kiểm tra xem người dùng có phải là Admin hoặc Shipper không
            if (await _userManager.IsInRoleAsync(user, "Admin") || await _userManager.IsInRoleAsync(user, "Shipper"))
            {
                var result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    TempData["SuccessMessage"] = "Xóa người dùng thành công";
                    return RedirectToAction(nameof(UserList));
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    TempData["ErrorMessage"] = "Có lỗi xảy ra khi xóa người dùng";
                    return RedirectToAction(nameof(UserList));
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không có quyền xóa người dùng này";
                return RedirectToAction(nameof(UserList));
            }
        }


        public async Task<IActionResult> CreateUserWithRole()
        {
            return View(new CreateUserWithRoleViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateUserWithRole(CreateUserWithRoleViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            if (model.Password != model.ConfirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Mật khẩu và mật khẩu xác nhận không khớp!");
                return View(model);
            }

            var user = new User { UserName = model.Email, Email = model.Email, PhoneNumber = model.PhoneNumber };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                // Gán vai trò tùy thuộc vào giá trị roleName
                if (model.RoleName == "Shipper")
                {
                    await _userManager.AddToRoleAsync(user, RoleList.Shipper);
                }
                else if (model.RoleName == "Admin")
                {
                    await _userManager.AddToRoleAsync(user, RoleList.Admin);
                }
                else if (model.RoleName == "Salesperson")
                {
                    await _userManager.AddToRoleAsync(user, RoleList.Salesperson);
                }
                // Mặc định gán vai trò Customer
                await _userManager.AddToRoleAsync(user, RoleList.Customer);

                // Cập nhật email và xác nhận email ngay lập tức
                await _userManager.SetEmailAsync(user, model.Email);
                user.EmailConfirmed = true;
                var updateResult = await _userManager.UpdateAsync(user);
                if (!updateResult.Succeeded)
                {
                    foreach (var error in updateResult.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return View(model);
                }
                TempData["SuccessMessage"] = "Tạo người dùng thành công!";
                return RedirectToAction("CreateUserWithRole", "AdminHome");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
                TempData["ErrorMessage"] = "Tạo người dùng thất bại. Vui lòng kiểm tra lại thông tin.";

                return View(model);
            }
        }
    }

    public class CreateUserWithRoleViewModel
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email phỉa đúng định dạng của email")]
        [Display(Name = "Địa chỉ email của bạn")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu là bắt buộc")]
        [StringLength(100, ErrorMessage = "{0} phải dài ít nhất là {2} và tối đa {1} ký tự.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Mật khẩu")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Quyền người dùng là bắt buộc")]
        public string RoleName { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [Phone]
        public string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu là bắt buộc")]
        [DataType(DataType.Password)]
        [Display(Name = "Nhập lại mật khẩu")]
        [Compare("Password", ErrorMessage = "Mật khẩu và mật khẩu xác nhận không khớp!")]
        public string ConfirmPassword { get; set; }
    }
}
