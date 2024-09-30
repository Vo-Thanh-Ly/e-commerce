using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{

    public partial class Invoice
    {
        [Key]
        public int InvoiceId { get; set; }

        [Required]
        [StringLength(450)]
        public string CustomerId { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? InvoiceDate { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal TotalAmount { get; set; }

        public int StatusId { get; set; }

        [StringLength(500)]
        public string Notes { get; set; }

        public int? PaymentId { get; set; }

        [InverseProperty("Invoice")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

        [ForeignKey("PaymentId")]
        [InverseProperty("Invoices")]
        public virtual Payment Payment { get; set; }

        //[InverseProperty("Invoice")]
        //public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();

        [ForeignKey("StatusId")]
        [InverseProperty("Invoices")]
        public virtual InvoiceStatus Status { get; set; }

        [ForeignKey("CustomerId")]
        public virtual User User { get; set; }
    }

}
