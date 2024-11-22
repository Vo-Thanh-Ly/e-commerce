namespace Sell_​_cleaning_services_e_commerce.Models.VnPayViewModel
{

    public class VnPaymentRequestModel
    {
        public int OrderId { get; set; }
        public string FullName { get; set; }
        public string Description { get; set; }
        public double Amount { get; set; }
        public DateTime CreatedDate { get; set; }

        public string Notes { get; set; }  // Ghi chú
        public string FullNameInInvoice { get; set; }  // Tên đầy đủ
        public string AddressInInvoice { get; set; }  // Địa chỉ
        public string PhoneNumberInInvoice { get; set; }  // Số điện thoại
    }
}
