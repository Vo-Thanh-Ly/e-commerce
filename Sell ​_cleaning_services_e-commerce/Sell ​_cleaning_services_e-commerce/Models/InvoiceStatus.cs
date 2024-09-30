using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    [Table("InvoiceStatus")]
    public partial class InvoiceStatus
    {
        [Key]
        public int StatusId { get; set; }

        [Required]
        [StringLength(50)]
        public string StatusName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [InverseProperty("Status")]
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
