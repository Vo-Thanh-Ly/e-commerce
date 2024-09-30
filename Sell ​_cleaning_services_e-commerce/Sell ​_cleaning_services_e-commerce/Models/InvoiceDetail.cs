using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public partial class InvoiceDetail
    {
        [Key]
        public int InvoiceDetailId { get; set; }

        public int InvoiceId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal UnitPrice { get; set; }

        [Column(TypeName = "decimal(18, 2)")]
        public decimal Total { get; set; }

        [ForeignKey("InvoiceId")]
        [InverseProperty("InvoiceDetails")]
        public virtual Invoice Invoice { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("InvoiceDetails")]
        public virtual Product Product { get; set; }
    }
}
