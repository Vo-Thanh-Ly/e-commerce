using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Models;

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
        //Xem chi tiết đơn hàng
        public async Task<IActionResult> OrderDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //Lấy id hóa đơn từ database
            int id01 = _context.Invoices.Where(i => i.InvoiceId == id).FirstOrDefault().InvoiceId;
            // Lấy chi tiết hóa đơn từ database
            List<InvoiceDetail> invoiceDetails = await _context.InvoiceDetails.Include(p => p.Product)
                                                               .Where(i => i.InvoiceId == id01)
                                                               .ToListAsync();  // Dùng ToListAsync để làm việc bất đồng bộ

            int sc = 0;
            int sc01 = _context.Invoices.Where(i => i.InvoiceId == id).FirstOrDefault().StatusId;

            if (sc01 == 1)
            {
                sc = 1;
            }


            ViewBag.sc = sc;
            ViewBag.id = id;
            if (invoiceDetails == null || !invoiceDetails.Any())
            {
                return NotFound();  // Trả về 404 nếu không tìm thấy chi tiết hóa đơn
            }

            double sum = invoiceDetails.Sum(i => i.Total);
            ViewBag.sum = sum;
            return View(invoiceDetails);
        }

        //Hủy bỏ đơn giao không nhận hàng
        public async Task<IActionResult> FailedDelivery(int id, string liDo)
        {
            // Tìm kiếm thanh toán theo id
            var payment = await _context.Payments.FindAsync(id);

            // Kiểm tra nếu payment không tồn tại
            if (payment == null)
            {
                return NotFound("Không tìm thấy thanh toán."); // Trả về NotFound nếu không tìm thấy thanh toán
            }

            // Cập nhật trạng thái của thanh toán thành hủy
            payment.Canceled = true;
            payment.DeliveryFailed = true;

            // Tìm kiếm log liên quan đến thanh toán
            var paymentLogs = await _context.PaymentLogs
                                   .Where(p => p.PaymentId == payment.PaymentId)
                                   .FirstOrDefaultAsync();

            // Kiểm tra nếu paymentLogs không tồn tại và khởi tạo mới nếu cần
            if (paymentLogs == null)
            {
                paymentLogs = new PaymentLog
                {
                    PaymentId = payment.PaymentId,
                    LogData = liDo,
                    LogDate = DateTime.Now // Thêm các thuộc tính cần thiết
                };

                // Thêm log mới vào context
                _context.PaymentLogs.Add(paymentLogs);
            }
            else
            {
                // Cập nhật lý do thất bại cho log hiện có
                paymentLogs.LogData = liDo;
            }

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Quay lại trang danh sách hoặc trang chính của shipper
            return Json(new { success = true });
        }


        // Action cho Giao hàng và nhận tiền thành công
        public async Task<IActionResult> CompleteDelivery(int id)
        {
            // Tìm kiếm thanh toán theo id
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy thanh toán
            }

            // Tìm kiếm hóa đơn theo InvoiceId và cập nhật trạng thái
            var invoice = await _context.Invoices.FindAsync(payment.InvoiceId);
            if (invoice != null)
            {
                invoice.StatusId = 2; // Cập nhật trạng thái của hóa đơn thành "hoàn thành"
            }

            // Cập nhật trạng thái giao hàng và ngày thanh toán
            payment.DeliveredSussced = true;
            payment.PaymentDate = DateTime.Now;

            // Tìm kiếm hoặc khởi tạo mới log liên quan đến thanh toán
            var paymentLogs = await _context.PaymentLogs
                                   .Where(p => p.PaymentId == payment.PaymentId)
                                   .FirstOrDefaultAsync();

            // Kiểm tra nếu log không tồn tại và tạo mới nếu cần
            if (paymentLogs == null)
            {
                paymentLogs = new PaymentLog
                {
                    PaymentId = payment.PaymentId,
                    LogData = "Giao hàng thành công và đã nhận tiền hàng",
                    LogDate = DateTime.Now // Thêm thuộc tính cần thiết
                };

                // Thêm log mới vào context
                _context.PaymentLogs.Add(paymentLogs);
            }
            else
            {
                // Cập nhật nội dung log nếu đã tồn tại
                paymentLogs.LogData = "Giao hàng thành công và đã nhận tiền hàng";
            }

            // Lưu tất cả thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Quay lại danh sách hoặc trang chính của shipper
            return RedirectToAction("Shipper_Index");
        }



        // Action cho Giao thành công(đã thanh toán online)
        public async Task<IActionResult> SuccessfulDelivery(int id)
        {
            // Tìm kiếm thanh toán theo id
            var payment = await _context.Payments.FindAsync(id);
            if (payment == null)
            {
                return NotFound(); // Trả về NotFound nếu không tìm thấy thanh toán
            }

            // Tìm kiếm hoặc khởi tạo mới payment log liên quan đến thanh toán
            var paymentLogs = await _context.PaymentLogs
                                  .Where(p => p.PaymentId == payment.PaymentId)
                                  .FirstOrDefaultAsync();

            // Nếu paymentLogs không tồn tại, tạo mới nó
            if (paymentLogs == null)
            {
                paymentLogs = new PaymentLog
                {
                    PaymentId = payment.PaymentId,
                    LogData = "Giao hàng thành công",
                    LogDate = DateTime.Now // Thêm thuộc tính ngày tạo nếu cần
                };

                // Thêm log mới vào context
                _context.PaymentLogs.Add(paymentLogs);
            }
            else
            {
                // Cập nhật nội dung log nếu log đã tồn tại
                paymentLogs.LogData = "Giao hàng thành công";
            }

            // Cập nhật trạng thái giao hàng thành công
            payment.DeliveredSussced = true;

            // Lưu thay đổi vào cơ sở dữ liệu
            await _context.SaveChangesAsync();

            // Quay lại danh sách hoặc trang chính của shipper
            return RedirectToAction("Shipper_Index");
        }


        // Action cho Giao thất bại
        //public async Task<IActionResult> FailedDelivery(int id)
        //{
        //    var payment = await _context.Payments.FindAsync(id);
        //    if (payment == null)
        //    {
        //        return NotFound();
        //    }

        //    // Logic để xử lý giao hàng thất bại cập nhật giao thất  bại thành true
        //    payment.DeliveryFailed = true;
        //    _context.SaveChanges();

        //    // Trả về view để hiển thị thông tin chi tiết về giao hàng thất bại
        //    return RedirectToAction("Shipper_Index"); // Quay lại danh sách hoặc trang chính của shipper
        //}


    }
}
