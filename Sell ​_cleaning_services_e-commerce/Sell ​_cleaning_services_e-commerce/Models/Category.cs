using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    [Table("Category")]
    public partial class Category
    {
        [Key]
        public int CategoryId { get; set; }

        [Required]
        [StringLength(100)]
        public string CategoryName { get; set; }

        [InverseProperty("Category")]
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
