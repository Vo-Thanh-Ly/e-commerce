using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    [Table("ShoppingCart")]
    public partial class ShoppingCart
    {
        [Key]
        public int CartId { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? DateAdded { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ShoppingCarts")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }

}
