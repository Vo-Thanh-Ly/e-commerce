using Sell__cleaning_services_e_commerce.Helpers;
using Sell__cleaning_services_e_commerce.Models.VnPayViewModel;

namespace Sell_​_cleaning_services_e_commerce.Services
{
    public class VnPayService : IVnPayService
    {
        private readonly IConfiguration _config;

        public VnPayService(IConfiguration config)
        {
            _config = config;
        }

        public string CreatePaymentUrl(HttpContext context, VnPaymentRequestModel model)
        {
            var tick = DateTime.Now.Ticks.ToString();

            var vnpay = new VnPayLibrary();
            vnpay.AddRequestData("vnp_Version", _config["VnPay:Version"]);
            vnpay.AddRequestData("vnp_Command", _config["VnPay:Command"]);
            vnpay.AddRequestData("vnp_TmnCode", _config["VnPay:TmnCode"]);
            vnpay.AddRequestData("vnp_Amount", (model.Amount * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000

            vnpay.AddRequestData("vnp_CreateDate", model.CreatedDate.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", _config["VnPay:CurrCode"]);
            vnpay.AddRequestData("vnp_IpAddr", Utils.GetIpAddress(context));
            vnpay.AddRequestData("vnp_Locale", _config["VnPay:Locale"]);

            var orderInfo = $"OrderId: {model.OrderId}; " +
                $"Notes: {model.Notes}; " +
                $"FullNameInInvoice: {model.FullNameInInvoice}; " +
                $"AddressInInvoice: {model.AddressInInvoice}; " +
                $"PhoneNumberInInvoice: {model.PhoneNumberInInvoice}";

            vnpay.AddRequestData("vnp_OrderInfo", orderInfo);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other
            vnpay.AddRequestData("vnp_ReturnUrl", _config["VnPay:PaymentBackReturnUrl"]);

            vnpay.AddRequestData("vnp_TxnRef", tick); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            var paymentUrl = vnpay.CreateRequestUrl(_config["VnPay:BaseUrl"], _config["VnPay:HashSecret"]);

            return paymentUrl;
        }

        public VnPaymentResponseModel PaymentExecute(IQueryCollection collections)
        {
            var vnpay = new VnPayLibrary();
            foreach (var (key, value) in collections)
            {
                if (!string.IsNullOrEmpty(key) && key.StartsWith("vnp_"))
                {
                    vnpay.AddResponseData(key, value.ToString());
                }
            }

            var vnp_orderId = Convert.ToInt64(vnpay.GetResponseData("vnp_TxnRef"));
            var vnp_TransactionId = Convert.ToInt64(vnpay.GetResponseData("vnp_TransactionNo"));
            var vnp_SecureHash = collections.FirstOrDefault(p => p.Key == "vnp_SecureHash").Value;
            var vnp_ResponseCode = vnpay.GetResponseData("vnp_ResponseCode");
            var vnp_OrderInfo = vnpay.GetResponseData("vnp_OrderInfo");
            //-----------------------------
            var vnp_PayDate = vnpay.GetResponseData("vnp_PayDate");
            var vnp_BankCode = vnpay.GetResponseData("vnp_BankCode");
            var vnp_BankTranNo = vnpay.GetResponseData("vnp_BankTranNo");
            var vnp_Amount = vnpay.GetResponseData("vnp_Amount");


            bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, _config["VnPay:HashSecret"]);
            if (!checkSignature)
            {
                return new VnPaymentResponseModel
                {
                    Success = false
                };
            }

            return new VnPaymentResponseModel
            {
                Success = true,
                PaymentMethod = "Thanh toán online qua VnPay",
                OrderDescription = vnp_OrderInfo,
                OrderId = vnp_orderId.ToString(),
                TransactionId = vnp_TransactionId.ToString(),
                Token = vnp_SecureHash,
                VnPayResponseCode = vnp_ResponseCode,
                BankCode = vnp_BankCode,
                PaymentDate = TryParsePaymentDate(vnp_PayDate), // Gọi hàm chuyển đổi
                TotalAmount = TryParseTotalAmount(vnp_Amount)
            };
        }

        private double TryParseTotalAmount(string vnp_Amount)
        {
            // Kiểm tra xem vnp_Amount có giá trị null hoặc rỗng hay không
            if (string.IsNullOrEmpty(vnp_Amount))
            {
                return 0; // Trả về 0 nếu chuỗi rỗng hoặc null
            }

            // Thử chuyển đổi chuỗi thành double
            if (double.TryParse(vnp_Amount, out double totalAmount))
            {
                return totalAmount; // Nếu thành công, trả về totalAmount
            }
            else
            {
                return 0; // Nếu thất bại, trả về 0 hoặc giá trị mặc định mà bạn muốn
            }
        }


        private DateTime? TryParsePaymentDate(string payDate)
        {
            // Thử chuyển đổi chuỗi thành DateTime
            if (DateTime.TryParse(payDate, out DateTime paymentDate))
            {
                return paymentDate; // Nếu thành công, trả về paymentDate
            }
            else
            {
                return DateTime.Now; // Nếu thất bại, trả về ngày hôm nay
            }
        }
    }
}
