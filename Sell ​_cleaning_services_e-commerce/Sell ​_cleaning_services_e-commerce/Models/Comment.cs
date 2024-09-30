using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{

    public partial class Comment
    {
        [Key]
        public int CommentId { get; set; }

        public int ProductId { get; set; }

        [Required]
        [StringLength(450)]
        public string UserId { get; set; }

        [Required]
        public string Content { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedDate { get; set; }

        public int? Rating { get; set; }

        [ForeignKey("ProductId")]
        [InverseProperty("Comments")]
        public virtual Product Product { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("Comments")]
        public virtual User User { get; set; }
    }

}
