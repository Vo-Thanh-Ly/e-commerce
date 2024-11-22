using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Areas.MailService;
using Sell__cleaning_services_e_commerce.Areas.Other;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Models;
using Sell__cleaning_services_e_commerce.Models.ViewModel;
using Sell__cleaning_services_e_commerce.Models.VnPayViewModel;
using Sell__cleaning_services_e_commerce.Services;
using System.Text;
using System.Text.RegularExpressions;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{
    public class ShoppingCartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IVnPayService _vnPayservice;
        private readonly ILogger<ShoppingCartController> _logger;
        private readonly IEmailSender _sender;

        public ShoppingCartController(
            ApplicationDbContext context,
            SignInManager<User> signInManager,
            UserManager<User> userManager,
            IVnPayService vnPayService,
            ILogger<ShoppingCartController> logger,
            IEmailSender emailSender)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
            _vnPayservice = vnPayService;
            _logger = logger;
            _sender = emailSender;
        }
       public int countShopping = 0;
        // Hiển thị giỏ hàng
        public async Task<IActionResult> Index()
        {

            var cartViewModel = new List<CartItemViewModel>();
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                cartViewModel = await _context.ShoppingCarts
                .Where(sc => sc.UserId == user.Id)
                .Select(sc => new CartItemViewModel
                {
                    ProductId = sc.ProductId,
                    ProductName = sc.Product.ProductName,
                    Quantity = sc.Quantity,
                    Price = sc.Product.Price
                })
                .ToListAsync();

            }
            else
            { // Lấy giỏ hàng từ session
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");

                // Nếu giỏ hàng trống, tạo một danh sách trống
                if (cart == null)
                {
                    cart = new List<CartItem>();
                }

                // Tạo danh sách view model để truyền tới view
                cartViewModel = cart.Select(item =>
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductId == item.ProductId);
                    return new CartItemViewModel
                    {
                        ProductId = item.ProductId,
                        ProductName = product?.ProductName ?? "Unknown",  // Kiểm tra null để tránh lỗi
                        Quantity = item.Quantity,
                        Price = product?.Price ?? 0,  // Nếu product null thì giá mặc định là 0
                        MaxQuantity = _context.Products
                                        .Where(p => p.ProductId == item.ProductId)
                                        .Select(p => p.QuantityInStock)
                                        .FirstOrDefault()
                    };
                }).ToList();
            }
            return View(cartViewModel);
        }

        public async Task<IActionResult> GetCartItemCount()
        {
            int itemCount = 0;

            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                itemCount = await _context.ShoppingCarts
                    .Where(sc => sc.UserId == user.Id)
                    .SumAsync(sc => sc.Quantity);
            }
            else
            {
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart");
                itemCount = cart?.Sum(item => item.Quantity) ?? 0;
            }

            return Json(new { count = itemCount });
        }



        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                var existingItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.UserId == user.Id && sc.ProductId == productId);

                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
                    if (product != null)
                    {
                        _context.ShoppingCarts.Add(new ShoppingCart
                        {
                            UserId = user.Id,
                            ProductId = productId,
                            Quantity = quantity
                        });
                    }
                }

                await _context.SaveChangesAsync();
            }
            else
            {
                // Lấy giỏ hàng từ session, nếu không có thì tạo mới
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();

                // Kiểm tra xem sản phẩm đã tồn tại trong giỏ hàng chưa
                var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);
                if (existingItem != null)
                {
                    existingItem.Quantity += quantity;
                }
                else
                {
                    var product = _context.Products.FirstOrDefault(p => p.ProductId == productId);
                    if (product != null)
                    {
                        cart.Add(new CartItem
                        {
                            ProductId = productId,
                            Quantity = quantity,
                            Price = product.Price // Gán giá sản phẩm
                        });
                    }
                }

                // Lưu lại giỏ hàng vào session
                HttpContext.Session.SetObjectAsJson("Cart", cart);

                // Trả về phản hồi JSON (hoặc một thông điệp)
                return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" }); ;
            }

            return Json(new { success = true, message = "Sản phẩm đã được thêm vào giỏ hàng!" });
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCartItem(int productId, int quantity)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Xử lý cho người dùng đã đăng nhập
                var user = await _userManager.GetUserAsync(User);
                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.UserId == user.Id && sc.ProductId == productId);

                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }
            else
            {
                // Xử lý cho người dùng chưa đăng nhập (sử dụng session)
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
                var item = cart.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    item.Quantity = quantity;
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                    return Json(new { success = true });
                }
                return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
            }
        }


        [HttpPost]
        public async Task<IActionResult> RemoveFromCart(int productId)
        {
            if (User.Identity.IsAuthenticated)
            {
                // Xử lý cho người dùng đã đăng nhập
                var user = await _userManager.GetUserAsync(User);
                var cartItem = await _context.ShoppingCarts
                    .FirstOrDefaultAsync(sc => sc.UserId == user.Id && sc.ProductId == productId);

                if (cartItem != null)
                {
                    _context.ShoppingCarts.Remove(cartItem);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true });
                }
            }
            else
            {
                // Xử lý cho người dùng chưa đăng nhập (sử dụng session)
                var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>("Cart") ?? new List<CartItem>();
                var item = cart.FirstOrDefault(i => i.ProductId == productId);
                if (item != null)
                {
                    cart.Remove(item);
                    HttpContext.Session.SetObjectAsJson("Cart", cart);
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, message = "Không tìm thấy sản phẩm trong giỏ hàng" });
        }

        [Authorize(Roles = RoleList.Customer)]
        [HttpGet]
        public async Task<IActionResult> Checkout()
        {

            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");

            var user = await _userManager.GetUserAsync(User);
            Boolean usePersonalInfor=false;
            if (user.Nomalname == null || user.Address == null || user.PhoneNumber == null)
            {
                // Tạo một chuỗi HTML và gán nó vào ViewBag
                string htmlContent = @"
                    <div class='form-group mb-3'>
                        <div class='border-1 border-info'>
                            <p>
                                Nếu bạn muốn sử dụng thông tin mặc định của tài khoản, 
                                bạn vui lòng cập nhật thông tin 
                                <a class='text-lg' href='/Identity/Account/Manage'>tại đây</a>
                            </p>
                        </div>
                    </div>";
                // Gửi mã HTML đến View thông qua ViewBag
                ViewBag.HtmlContent = htmlContent;
            }
           
            else if (user.Nomalname != null && user.Address != null && user.PhoneNumber != null)
            {
                usePersonalInfor = true;
            }
            ViewBag.usePersonalInfor = usePersonalInfor;

            // Add a checkbox to the view model
            ViewData["UseDefaultUserInfo"] = new SelectList(new[] { new { Value = true, Text = "Sử dụng thông tin tài khoản" } }, "Value", "Text");
            List<CartItemViewModel> cartViewModel = await _context.ShoppingCarts
              .Where(sc => sc.UserId == user.Id)
              .Select(sc => new CartItemViewModel
              {
                  ProductId = sc.ProductId,
                  ProductName = sc.Product.ProductName,
                  Quantity = sc.Quantity,
                  Price = sc.Product.Price
              }).ToListAsync();
            if (cartViewModel.Count == 0)
            {
                return Redirect("/");
            }


            return View(cartViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Checkout(string HoTen, string DienThoai, string DiaChi, string GhiChu, int PaymentMethodId = 2)
        {

            CheckoutVM model = new CheckoutVM();
            model.DiaChi = DiaChi;
            model.HoTen = HoTen;
            model.DienThoai = DienThoai;
            model.GhiChu = GhiChu;

            if (bool.TryParse(Request.Form["UseDefaultUserInfo"], out bool useDefaultUserInfo) && useDefaultUserInfo)
            {
                var user = await _userManager.GetUserAsync(User);
                model.HoTen = user.UserName;
                model.DiaChi = user.Address;
                model.DienThoai = user.PhoneNumber;
                if (model.HoTen == null || model.DiaChi == null || model.DienThoai == null)
                {
                    return Redirect("~Areas/Account/Manage/Index.cshtml");
                }
            }


            if (ModelState.IsValid)
            {
                try
                {
                    var user = await _userManager.GetUserAsync(User);
                    var cartItems = await _context.ShoppingCarts
                        .Include(sc => sc.Product)
                        .Where(sc => sc.UserId == user.Id)
                        .ToListAsync();

                    System.Diagnostics.Debug.WriteLine($"Cart Items Count: {cartItems.Count}");

                    if (!cartItems.Any())
                    {
                        TempData["CheckOutResusal"] = "Giỏ hàng của bạn không có sản phẩm nào cả";
                        return RedirectToAction("Index", "ShoppingCart");
                    }

                    // Tính tổng tiền
                    double sum = cartItems.Sum(item => item.Quantity * item.Product.Price);
                    System.Diagnostics.Debug.WriteLine($"Total Amount: {sum}");

                    // Xử lý thanh toán VNPay
                    if (PaymentMethodId == 1)
                    {

                        var vnPayModel = new VnPaymentRequestModel
                        {
                            Amount = sum,
                            CreatedDate = DateTime.Now,
                            Description = $"{model.HoTen} {model.DienThoai}",
                            FullName = model.HoTen,
                            OrderId = new Random().Next(1000, 100000),
                            AddressInInvoice = model.DiaChi,
                            FullNameInInvoice = model.HoTen,
                            Notes = model.GhiChu,
                            PhoneNumberInInvoice = model.DienThoai
                        };
                        return Redirect(_vnPayservice.CreatePaymentUrl(HttpContext, vnPayModel));
                    }
                    // Xử lý thanh toán khi nhận hàng
                    else if (PaymentMethodId == 2)
                    {
                        System.Diagnostics.Debug.WriteLine("Processing Cash on Delivery payment...");

                        // Khởi tạo transaction
                        using var transaction = await _context.Database.BeginTransactionAsync();
                        try
                        {
                            var customerId = user.Id;
                            System.Diagnostics.Debug.WriteLine($"Customer ID: {customerId}");

                            var khachHang = model.GiongKhachHang ? user : new User();

                            // Tạo hóa đơn mới
                            var hoadon = new Invoice
                            {
                                CustomerId = customerId,
                                FullName = model.HoTen ?? khachHang.UserName,
                                Address = model.DiaChi ?? khachHang.Address,
                                PhoneNumber = model.DienThoai,
                                InvoiceDate = DateTime.Now,
                                StatusId = 2,
                                Notes = model.GhiChu,
                                TotalAmount = sum
                            };

                            System.Diagnostics.Debug.WriteLine($"Creating invoice for customer: {hoadon.FullName}");
                            _context.Invoices.Add(hoadon);
                            await _context.SaveChangesAsync();
                            System.Diagnostics.Debug.WriteLine($"Invoice created with ID: {hoadon.InvoiceId}");

                            // Tạo payment
                            var payment = new Payment
                            {
                                InvoiceId = hoadon.InvoiceId,
                                PaymentMethodId = PaymentMethodId,
                                DeliveryFailed = false,
                                Canceled = false,
                                PaymentDate = DateTime.Now,
                                TransactionId = Guid.NewGuid().ToString(),
                                DeliveredSussced = false,
                                PhoneNumber = hoadon.PhoneNumber,
                                Note = "Thanh toán hóa đơn mới",
                                Address = hoadon.Address ?? user.Address,
                                BankCode = "DEFAULT",
                                ResponseCode = "00"
                            };

                            System.Diagnostics.Debug.WriteLine($"Creating payment for invoice: {hoadon.InvoiceId}");
                            _context.Payments.Add(payment);
                            await _context.SaveChangesAsync();
                            System.Diagnostics.Debug.WriteLine("Payment created successfully");

                            // Tạo chi tiết hóa đơn từ giỏ hàng
                            var invoiceDetails = new List<InvoiceDetail>();
                            foreach (var item in cartItems)
                            {
                                var detail = new InvoiceDetail
                                {
                                    InvoiceId = hoadon.InvoiceId,
                                    ProductId = item.ProductId,
                                    Quantity = item.Quantity,
                                    UnitPrice = item.Product.Price,
                                    Total = (item.Quantity * item.Product.Price)
                                };
                                invoiceDetails.Add(detail);
                                System.Diagnostics.Debug.WriteLine($"Adding invoice detail for product: {item.ProductId}");
                            }

                            _context.InvoiceDetails.AddRange(invoiceDetails);
                            await _context.SaveChangesAsync();
                            System.Diagnostics.Debug.WriteLine("Invoice details created successfully");
                            await SendOrderConfirmationEmailAsync(user, hoadon, invoiceDetails, "Thanh toán khi nhận hàng", hoadon.Notes);
                            // Xóa giỏ hàng
                            System.Diagnostics.Debug.WriteLine($"Removing {cartItems.Count} items from shopping cart");
                            _context.ShoppingCarts.RemoveRange(cartItems);
                            await _context.SaveChangesAsync();
                            System.Diagnostics.Debug.WriteLine("Shopping cart items removed successfully");

                            // Commit transaction
                            await transaction.CommitAsync();
                            System.Diagnostics.Debug.WriteLine("Transaction committed successfully");

                            // Xóa session giỏ hàng
                            HttpContext.Session.Clear();
                            TempData["CheckOutResusal"] = "Checkout thành công";
                            System.Diagnostics.Debug.WriteLine("Checkout completed successfully");

                            return RedirectToAction("Index", "ShoppingCart");
                        }
                        catch (Exception ex)
                        {
                            await transaction.RollbackAsync();
                            System.Diagnostics.Debug.WriteLine($"Error during checkout: {ex.Message}");
                            System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");

                            // Có thể thêm log vào database hoặc file
                            TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý đơn hàng. Vui lòng thử lại sau.";

                            return RedirectToAction("Index", "ShoppingCart");
                        }
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine($"Invalid PaymentMethodId: {PaymentMethodId}");
                        TempData["Error"] = "Phương thức thanh toán không hợp lệ";
                        return RedirectToAction("Index", "ShoppingCart");
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Unexpected error: {ex.Message}");
                    System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
                    TempData["Error"] = "Có lỗi xảy ra. Vui lòng thử lại sau.";
                    return RedirectToAction("Index", "ShoppingCart");
                }
            }

            // If we got this far, something failed, redisplay form
            ViewData["PaymentMethodId"] = new SelectList(_context.PaymentMethods, "PaymentMethodId", "MethodName");
            return View(model);
        }

        public async Task<IActionResult> PaymentCallBack()
        {
            var response = _vnPayservice.PaymentExecute(Request.Query);

            // Kiểm tra nếu response null hoặc mã phản hồi khác "00" (không thành công)
            if (response == null || response.VnPayResponseCode != "00")
            {
                TempData["Message"] = $"Lỗi thanh toán VN Pay: {response?.VnPayResponseCode}";
                return RedirectToAction("PaymentFail");
            }

            // Khởi tạo transaction
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Lấy ID khách hàng và thông tin user
                var user = await _userManager.GetUserAsync(User);
                var customerId = user.Id;

                // Phân giải thông tin response
                var parsedData = ParseOrderDescriptionToVnPayResponse(response.OrderDescription);

                // Tạo hóa đơn
                var hoadon = new Invoice
                {
                    CustomerId = customerId,
                    FullName = parsedData.FullNameInInvoice,
                    Address = parsedData.AddressInInvoice,
                    PhoneNumber = parsedData.PhoneNumberInInvoice,
                    InvoiceDate = DateTime.Now,
                    StatusId = 1,
                    Notes = parsedData.Notes,
                    TotalAmount = response.TotalAmount
                };

                _context.Invoices.Add(hoadon);
                await _context.SaveChangesAsync();

                // Tạo thanh toán
                var payment = new Payment
                {
                    InvoiceId = hoadon.InvoiceId,
                    PaymentMethodId = 1,  // VNPay
                    DeliveryFailed = false,
                    Canceled = false,
                    PaymentDate = DateTime.Now,
                    TransactionId = response.TransactionId,
                    DeliveredSussced = false,
                    PhoneNumber = hoadon.PhoneNumber,
                    Note = "Thanh toán qua VNPay",
                    Address = hoadon.Address,
                    BankCode = response.BankCode,
                    ResponseCode = response.VnPayResponseCode
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                // Lấy danh sách sản phẩm từ giỏ hàng
                var cartItems = await _context.ShoppingCarts
                    .Where(c => c.UserId == customerId)
                    .Include(c => c.Product)
                    .ToListAsync();

                // Tạo chi tiết hóa đơn từ giỏ hàng
                var invoiceDetails = new List<InvoiceDetail>();
                foreach (var item in cartItems)
                {
                    var detail = new InvoiceDetail
                    {
                        InvoiceId = hoadon.InvoiceId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        UnitPrice = item.Product.Price,
                        Total = (item.Quantity * item.Product.Price)
                    };
                    invoiceDetails.Add(detail);
                }

                _context.InvoiceDetails.AddRange(invoiceDetails);
                await _context.SaveChangesAsync();

                await SendOrderConfirmationEmailAsync(user, hoadon, invoiceDetails, "Đã thanh toán", hoadon.Notes);


                // Xóa giỏ hàng
                _context.ShoppingCarts.RemoveRange(cartItems);
                await _context.SaveChangesAsync();

                // Commit transaction
                await transaction.CommitAsync();

                // Xóa session giỏ hàng
                HttpContext.Session.Clear();
                TempData["CheckOutResusal"] = "Checkout thành công qua VNPay";

                return RedirectToAction("Index", "ShoppingCart");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                TempData["Error"] = "Có lỗi xảy ra trong quá trình xử lý đơn hàng. Vui lòng thử lại sau.";
                return RedirectToAction("Index", "ShoppingCart");
            }
        }

        public async Task SendOrderConfirmationEmailAsync(User user, Invoice hoadon, List<InvoiceDetail> invoiceDetails, string status, string note)
        {
            var emailContent = new StringBuilder();

            // Nội dung email HTML
            emailContent.AppendLine("<html>");
            emailContent.AppendLine("<body>");
            emailContent.AppendLine($"<h2>Kính chào {hoadon.FullName},</h2>");
            emailContent.AppendLine("<p>Cảm ơn bạn đã đặt hàng! Dưới đây là thông tin chi tiết về đơn hàng của bạn:</p>");
            emailContent.AppendLine("<h3>Thông tin đơn hàng:</h3>");
            emailContent.AppendLine("<table style='width:100%; border-collapse: collapse;'>");
            emailContent.AppendLine("<tr><td><strong>Mã đơn hàng:</strong></td><td>" + hoadon.InvoiceId + "</td></tr>");
            emailContent.AppendLine("<tr><td><strong>Ngày đặt hàng:</strong></td><td>" + hoadon.InvoiceDate.ToString() + "</td></tr>");
            emailContent.AppendLine("<tr><td><strong>Địa chỉ giao hàng:</strong></td><td>" + hoadon.Address + "</td></tr>");
            emailContent.AppendLine("<tr><td><strong>Số điện thoại:</strong></td><td>" + hoadon.PhoneNumber + "</td></tr>");
            emailContent.AppendLine("<tr><td><strong>Ghi chú:</strong></td><td>" + note + "</td></tr>");
            emailContent.AppendLine("</table>");

            // Danh sách sản phẩm
            emailContent.AppendLine("<h3>Sản phẩm:</h3>");
            emailContent.AppendLine("<table style='width:100%; border-collapse: collapse; border: 1px solid #ddd;'>");
            emailContent.AppendLine("<tr style='background-color: #f2f2f2;'>");
            emailContent.AppendLine("<th style='padding: 8px; text-align: left;'>Tên sản phẩm</th>");
            emailContent.AppendLine("<th style='padding: 8px; text-align: left;'>Số lượng</th>");
            emailContent.AppendLine("<th style='padding: 8px; text-align: right;'>Giá mỗi sản phẩm</th>");
            emailContent.AppendLine("</tr>");

            foreach (var item in invoiceDetails)
            {
                emailContent.AppendLine("<tr>");
                emailContent.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd;'>{item.Product.ProductName}</td>");
                emailContent.AppendLine($"<td style='padding: 8px; border: 1px solid #ddd;'>{item.Quantity}</td>");
                emailContent.AppendLine($"<td style='padding: 8px; text-align: right; border: 1px solid #ddd;'>{item.UnitPrice.ToString("N0")} VNĐ</td>");
                emailContent.AppendLine("</tr>");
            }
            emailContent.AppendLine("</table>");

            // Tổng tiền và trạng thái thanh toán
            emailContent.AppendLine("<h3>Tổng tiền:</h3>");
            emailContent.AppendLine("<p><strong>" + hoadon.TotalAmount.ToString("N0") + " VNĐ</strong></p>");
            emailContent.AppendLine("<h3>Trạng thái thanh toán:</h3>");
            emailContent.AppendLine("<p><strong>" + status + "</strong></p>");

            emailContent.AppendLine("<p>Xin cảm ơn bạn đã mua hàng tại cửa hàng của chúng tôi!</p>");
            emailContent.AppendLine("<p>Nếu có bất kỳ thắc mắc nào về đơn hàng của bạn, vui lòng liên hệ với chúng tôi qua email hoặc số điện thoại 123456789.</p>");

            emailContent.AppendLine("</body>");
            emailContent.AppendLine("</html>");

            // Gửi email
            await _sender.SendEmailAsync(
                user.Email,
                "Xác nhận đơn hàng của bạn",
                emailContent.ToString()
            );
        }

        private VnPaymentRequestModel ParseOrderDescriptionToVnPayResponse(string description)
        {
            var model = new VnPaymentRequestModel();

            // Sử dụng regex để phân giải các giá trị từ chuỗi
            var regex = new Regex(@"OrderId: (?<OrderId>\d+); Notes: (?<Notes>.*?); FullNameInInvoice: (?<FullNameInInvoice>.*?); AddressInInvoice: (?<AddressInInvoice>.*?); PhoneNumberInInvoice: (?<PhoneNumberInInvoice>\d+)");
            var match = regex.Match(description);

            if (match.Success)
            {
                model.OrderId = int.Parse(match.Groups["OrderId"].Value);
                model.Notes = match.Groups["Notes"].Value;
                model.FullNameInInvoice = match.Groups["FullNameInInvoice"].Value;
                model.AddressInInvoice = match.Groups["AddressInInvoice"].Value;
                model.PhoneNumberInInvoice = match.Groups["PhoneNumberInInvoice"].Value;
            }


            else
            {
                throw new InvalidOperationException("Không thể phân giải chuỗi OrderDescription.");
            }

            return model;
        }
    }
}
