using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public partial class PaymentMethod
    {
        [Key]
        public int PaymentMethodId { get; set; }

        [Required]
        [StringLength(100)]
        public string MethodName { get; set; }

        [StringLength(255)]
        public string Description { get; set; }

        [InverseProperty("PaymentMethod")]
        public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
