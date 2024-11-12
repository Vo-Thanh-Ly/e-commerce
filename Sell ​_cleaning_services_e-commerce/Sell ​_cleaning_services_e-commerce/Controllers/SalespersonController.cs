using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Areas.MailService;
using Sell__cleaning_services_e_commerce.Areas.Other;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Models;
using Sell__cleaning_services_e_commerce.Models.ViewModel;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{
    //  [Authorize(RoleList.Salesperson)]
    public class SalespersonController : Controller
    {
        private readonly IEmailSender _emailSender;
        private readonly UserManager<User> _userManager;
        private readonly ApplicationDbContext _context;

        public SalespersonController(IEmailSender emailSender, UserManager<User> userManager, ApplicationDbContext context)
        {
            _emailSender = emailSender;
            _userManager = userManager;
            _context = context;
        }
        public IActionResult Index()
        {

            return View(_context.EmailLogs.ToList());
        }

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailLog = await _context.EmailLogs
                .Include(e => e.Sender)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailLog == null)
            {
                return NotFound();
            }

            return View(emailLog);
        }

        public async Task<IActionResult> CreateEmailToOne()
        {

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmailToOne(EmailLogToOneView model)
        {
            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return View(model); // Nếu model không hợp lệ, trả về lại trang để người dùng sửa lỗi
            }

            // Gửi email
            await _emailSender.SendEmailAsync(model.ToEmail, model.Subject, model.Message);

            // Nếu gửi email thành công, lưu thông tin vào bảng EmailLogs
            var emailLog = new EmailLog
            {
                Subject = model.Subject,
                Message = model.Message,
                ToEmail = model.ToEmail,
                SentDate = DateTime.Now
            };

            // Lấy người dùng hiện tại
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                emailLog.SenderId = user.Id;  // Gán ID người gửi
            }

            // Lưu thông tin vào cơ sở dữ liệu
            _context.EmailLogs.Add(emailLog);
            await _context.SaveChangesAsync();

            // Chuyển hướng sau khi email được gửi và lưu thành công
            return RedirectToAction("Index");
        }



        public async Task<IActionResult> CreateEmailToAll()
        {

            return View();
        }

        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> CreateEmailToAll(EmailLogToAllView model)
        //{
        //    // Kiểm tra tính hợp lệ của model
        //    if (!ModelState.IsValid)
        //    {
        //        return View(model); // Nếu model không hợp lệ, trả về lại trang để người dùng sửa lỗi
        //    }

        //    // Gửi email
        //    List<User> users = _context.Users.Where(u => u.EmailConfirmed == true).ToList();
        //    foreach (User u in users)
        //    {
        //        await _emailSender.SendEmailAsync(u.Email, model.Subject, model.Message);
        //        // Nếu gửi email thành công, lưu thông tin vào bảng EmailLogs
        //        var emailLog = new EmailLog
        //        {
        //            Subject = model.Subject,
        //            Message = model.Message,
        //            ToEmail = u.Email,
        //            SentDate = DateTime.Now
        //        };

        //        // Lấy người dùng hiện tại
        //        var user = await _userManager.GetUserAsync(User);
        //        if (user != null)
        //        {
        //            emailLog.SenderId = user.Id;  // Gán ID người gửi
        //        }

        //        // Lưu thông tin vào cơ sở dữ liệu
        //        _context.EmailLogs.Add(emailLog);
        //    }




        //    await _context.SaveChangesAsync();

        //    // Chuyển hướng sau khi email được gửi và lưu thành công
        //    return RedirectToAction("Index");
        //}
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateEmailToAll(EmailLogToAllView model)
        {
            // Kiểm tra tính hợp lệ của model
            if (!ModelState.IsValid)
            {
                return View(model); // Nếu model không hợp lệ, trả về lại trang để người dùng sửa lỗi
            }

            // Lấy danh sách người dùng đã xác thực email từ cơ sở dữ liệu
            List<User> users = await _context.Users
                                              .Where(u => u.EmailConfirmed == true)
                                              .AsNoTracking() // Không theo dõi các đối tượng này để tiết kiệm bộ nhớ
                                              .ToListAsync();

            var emailTasks = new List<Task>(); // Danh sách các tác vụ gửi email

            // Lấy người dùng hiện tại (người gửi)
            var user = await _userManager.GetUserAsync(User);

            // Thêm mỗi email gửi vào danh sách các tác vụ không đồng bộ
            foreach (User u in users)
            {
                var sendEmailTask = _emailSender.SendEmailAsync(u.Email, model.Subject, model.Message);
                emailTasks.Add(sendEmailTask);

                // Lưu thông tin vào bảng EmailLogs
                var emailLog = new EmailLog
                {
                    Subject = model.Subject,
                    Message = model.Message,
                    ToEmail = u.Email,
                    SentDate = DateTime.Now,
                    SenderId = user?.Id // Gán ID người gửi (nếu người dùng hiện tại tồn tại)
                };

                _context.EmailLogs.Add(emailLog); // Lưu log vào cơ sở dữ liệu
            }

            // Chờ tất cả các email được gửi xong trước khi lưu vào cơ sở dữ liệu
            await Task.WhenAll(emailTasks);

            // Lưu tất cả các email log vào cơ sở dữ liệu một lần duy nhất
            await _context.SaveChangesAsync();

            // Chuyển hướng sau khi email được gửi và lưu thành công
            return RedirectToAction("Index");
        }




    }
}
