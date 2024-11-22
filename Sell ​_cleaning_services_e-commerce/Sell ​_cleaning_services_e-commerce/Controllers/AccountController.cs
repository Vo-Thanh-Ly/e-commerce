using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Sell__cleaning_services_e_commerce.Models;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;

        public AccountController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet]
        public async Task<IActionResult> GetCurrentUserInfo()
        {
            var user = await _userManager.GetUserAsync(User);

            if (user == null)
            {
                return BadRequest("User not found.");
            }

            var userInfo = new
            {
                userName = user.Nomalname,
                address = user.Address,
                phoneNumber = user.PhoneNumber
            };

            if (userInfo.userName == null || userInfo.address == null || userInfo.phoneNumber == null)
            {
                return Redirect("/Identity/Account/Manage/Index");
            }

            return Json(userInfo);
        }
    }
}
