using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public partial class ProductImage
    {
        [Key]
        public int ImageId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(300)]
        public string ImageUrl { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("ProductImages")]
        public virtual Product Product { get; set; }
    }

}
