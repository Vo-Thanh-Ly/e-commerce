namespace Sell_​_cleaning_services_e_commerce.Models.VnPayViewModel
{
    public class VnPaymentResponseModel
    {
        public bool Success { get; set; }
        public string PaymentMethod { get; set; }
        public string OrderDescription { get; set; }
        public string OrderId { get; set; }
        public string TransactionId { get; set; }
        public string Token { get; set; }
        public string VnPayResponseCode { get; set; }

        public double TotalAmount { get; set; }  // Tổng tiền hóa đơn
        public DateTime? PaymentDate { get; set; }  // Ngày thanh toán
        public string BankCode { get; set; }  // Mã ngân hàng

    }
}
