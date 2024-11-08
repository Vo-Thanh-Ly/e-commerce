using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{

    public partial class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }  // Khóa chính
        [Required]
        [StringLength(450)]
        public string CustomerId { get; set; }  // Khóa ngoại đến bảng người dùng

        [Column(TypeName = "datetime")]
        public DateTime? InvoiceDate { get; set; }  // Ngày lập hóa đơn


        public double TotalAmount { get; set; }  // Tổng tiền hóa đơn

        public int StatusId { get; set; }  // Khóa ngoại đến bảng trạng thái

        [StringLength(500)]
        public string Notes { get; set; }  // Ghi chú


        public virtual Payment Payment { get; set; }

        [ForeignKey("StatusId")]
        [InverseProperty("Invoices")]
        public virtual InvoiceStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User User { get; set; }

        [InverseProperty("Invoice")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
        public string? FullName { get; internal set; }
        public string? Address { get; internal set; }
        [Required]
        [Display(Name = "Số điện thoại")]
        [DataType(DataType.PhoneNumber)]
        public String PhoneNumber { get; internal set; }
    }

}
