using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public partial class Payment
    {
        [Key]
        public int PaymentId { get; set; }

        //  public int InvoiceId { get; set; }

        public int PaymentMethodId { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal PaymentAmount { get; set; }

        [Required]
        [StringLength(50)]
        public string PaymentStatus { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? PaymentDate { get; set; }

        [StringLength(100)]
        public string TransactionId { get; set; }

        [StringLength(10)]
        public string ResponseCode { get; set; }

        [StringLength(20)]
        public string BankCode { get; set; }

        [StringLength(255)]
        public string Note { get; set; }

        //[ForeignKey("InvoiceId")]
        //[InverseProperty("Payments")]
        //public virtual Invoice Invoice { get; set; }

        [InverseProperty("Payment")]
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();

        [InverseProperty("Payment")]
        public virtual ICollection<PaymentLog> PaymentLogs { get; set; } = new List<PaymentLog>();

        [ForeignKey("PaymentMethodId")]
        [InverseProperty("Payments")]
        public virtual PaymentMethod PaymentMethod { get; set; }
    }
}
