using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Data;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{

    [Authorize(Roles = "Shipper,Admin")]


    public class ShipperController : Controller
    {
        private ApplicationDbContext _context;
        public ShipperController(ApplicationDbContext context)
        {
            this._context = context;
        }
        //đon đang giao
        public async Task<IActionResult> Shipper_Index()
        {
            var applicationDbContext = _context.Payments.Include(p => p.Invoice).Include(p => p.Invoice.User).Include(p => p.PaymentMethod).Where(a => a.DeliveredSussced == false && a.DeliveryFailed == false);
            return View(await applicationDbContext.ToListAsync());
        }
        //Đơn giao thất bại 
        public async Task<IActionResult> GetUndeliveredOrders()
        {
            // Lấy tất cả các đơn giao thất bại
            var applicationDbContext = _context.Payments.Include(p => p.Invoice).Include(p => p.Invoice.User).Include(p => p.PaymentMethod).Where(a => a.DeliveredSussced == false && a.DeliveryFailed == true && a.Canceled == false);

            return View(await applicationDbContext.ToListAsync()); // Trả về view với danh sách đơn hàng chưa được giao
        }
        //Hủy bỏ đơn giao không nhận hàng
        public async Task<IActionResult> Cancel(int id)
        {
            // Tìm kiếm thanh toán theo id
            var payment = await _context.Payments.FindAsync(id);

            // Kiểm tra nếu payment không tồn tại
            if (payment == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy thanh toán
            }
            payment.Canceled = true;

            var paymentLogs = await _context.PaymentLogs
                                   .Where(p => p.PaymentId == payment.PaymentId)
                                   .FirstOrDefaultAsync(); // Lấy danh sách payment logs
            paymentLogs.LogData = "Giao hàng thất bại vì liên hệ nhiều lần không thành công";

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Quay lại trang danh sách hoặc trang chính của shipper
            return RedirectToAction("GetUndeliveredOrders");
        }

        // Action cho Giao hàng và nhận tiền thành công
        public async Task<IActionResult> CompleteDelivery(int id)
        {
            var payment = await _context.Payments.FindAsync(id); // Sử dụng FindAsync cho bất đồng bộ
            if (payment == null)
            {
                return NotFound();
            }

            // Tìm kiếm hóa đơn theo InvoiceId xác nhận thanh toán thanahf công cập nhật hôm nay thành ngày thanh toán
            var invoice = await _context.Invoices.FindAsync(payment.InvoiceId);
            invoice.StatusId = 2;

            // Logic để đánh dấu giao hàng và nhận tiền thành công
            payment.DeliveredSussced = true; // Cập nhật trạng thái giao hàng
            payment.PaymentDate = DateTime.Now;

            var paymentLogs = await _context.PaymentLogs
                                   .Where(p => p.PaymentId == payment.PaymentId)
                                   .FirstOrDefaultAsync(); // Lấy danh sách payment logs
            paymentLogs.LogData = "Giao hàng thành công và đã nhận tiền hàng";

            await _context.SaveChangesAsync(); // Lưu thay đổi một cách bất đồng bộ

            return RedirectToAction("Shipper_Index"); // Quay lại danh sách hoặc trang chính của shipper
        }


        // Action cho Giao thành công(đã thanh toán online)
        public async Task<IActionResult> SuccessfulDelivery(int id)
        {
            var payment = await _context.Payments.FindAsync(id); // Sử dụng FindAsync cho bất đồng bộ
            if (payment == null)
            {
                return NotFound();
            }

            var paymentLogs = await _context.PaymentLogs
                                  .Where(p => p.PaymentId == payment.PaymentId)
                                  .FirstOrDefaultAsync(); // Lấy danh sách payment logs
            paymentLogs.LogData = "Giao hàng thành công";

            // Logic để đánh dấu giao hàng và nhận tiền thành công
            payment.DeliveredSussced = true; // Cập nhật trạng thái giao hàng thành công

            await _context.SaveChangesAsync(); // Lưu thay đổi một cách bất đồng bộ

            return RedirectToAction("Shipper_Index"); // Quay lại danh sách hoặc trang chính của shipper
        }

        // Action cho Giao thất bại
        public async Task<IActionResult> FailedDelivery(int id)
        {
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound();
            }

            // Logic để xử lý giao hàng thất bại cập nhật giao thất  bại thành true
            payment.DeliveryFailed = true;
            _context.SaveChanges();

            // Trả về view để hiển thị thông tin chi tiết về giao hàng thất bại
            return RedirectToAction("Shipper_Index"); // Quay lại danh sách hoặc trang chính của shipper
        }


    }
}
