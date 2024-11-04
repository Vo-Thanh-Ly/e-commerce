using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public partial class Payment
    {
        [Key]
        [Display(Name = "Mã thanh toán")]
        public int PaymentId { get; set; }  // Khóa chính

        [Display(Name = "Mã hóa đơn")]
        public int InvoiceId { get; set; }  // Khóa ngoại đến Invoice

        [Display(Name = "Mã phương thức thanh toán")]
        public int PaymentMethodId { get; set; }

        [Required]
        [Display(Name = "Giao thất bại")]
        public Boolean DeliveryFailed { get; set; }

        [Required]
        [Display(Name = "Đã hủy")]
        public Boolean Canceled { get; set; }

        [Column(TypeName = "datetime")]
        [Display(Name = "Ngày thanh toán")]
        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        [Display(Name = "Mã giao dịch")]
        public string TransactionId { get; set; }

        [StringLength(10)]
        [Display(Name = "Mã phản hồi")]
        public string ResponseCode { get; set; }

        [StringLength(20)]
        [Display(Name = "Mã ngân hàng")]
        public string BankCode { get; set; }

        [Required]
        [Display(Name = "Trạng thái giao hàng")]
        public Boolean DeliveredSussced { get; set; }

        [Required]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public int PhoneNumber { get; set; }

        [StringLength(255)]
        [Display(Name = "Ghi chú")]
        public string Note { get; set; }

        [StringLength(300)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        [ForeignKey("InvoiceId")]
        [Display(Name = "Hóa đơn")]
        public virtual Invoice Invoice { get; set; }  // Navigation property đến Invoice

        [ForeignKey("PaymentMethodId")]
        [InverseProperty("Payments")]
        [Display(Name = "Phương thức thanh toán")]
        public virtual PaymentMethod PaymentMethod { get; set; }

        [InverseProperty("Payment")]
        [Display(Name = "Nhật ký thanh toán")]
        public virtual ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>();
    }
}
