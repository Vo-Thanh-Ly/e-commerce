using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public partial class Product
    {
        [Key]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Tên sản phẩm không được vượt quá 100 ký tự.")]
        [Display(Name = "Tên sản phẩm")]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Loại sản phẩm là bắt buộc.")]
        [Display(Name = "Loại sản phẩm")]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Mô tả sản phẩm là bắt buộc.")]
        [Display(Name = "Mô tả")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Giá sản phẩm là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Giá sản phẩm phải lớn hơn 0.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Giá")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng tồn kho phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Số lượng tồn kho")]
        public int QuantityInStock { get; set; }

        [Required(ErrorMessage = "Thương hiệu là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Thương hiệu không được vượt quá 100 ký tự.")]
        [Display(Name = "Thương hiệu")]
        public string Brand { get; set; }

        [Required(ErrorMessage = "Nhà cung cấp là bắt buộc.")]
        [Display(Name = "Nhà cung cấp")]
        public string Supplier { get; set; }

        [Required(ErrorMessage = "Trọng lượng là bắt buộc.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Trọng lượng phải lớn hơn 0.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Trọng lượng")]
        public decimal? Weight { get; set; }

        [Required(ErrorMessage = "Kích thước là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Kích thước không được vượt quá 50 ký tự.")]
        [Display(Name = "Kích thước")]
        public string Dimensions { get; set; }

        [Required(ErrorMessage = "Chất liệu là bắt buộc.")]
        [StringLength(50, ErrorMessage = "Chất liệu không được vượt quá 50 ký tự.")]
        [Display(Name = "Chất liệu")]
        public string Material { get; set; }

        [StringLength(50, ErrorMessage = "Công suất không được vượt quá 50 ký tự.")]
        [Display(Name = "Công suất")]
        public string Power { get; set; }

        [StringLength(50, ErrorMessage = "Nguồn năng lượng không được vượt quá 50 ký tự.")]
        [Display(Name = "Nguồn năng lượng")]
        public string PowerSource { get; set; }

        [Required(ErrorMessage = "Ngày sản xuất là bắt buộc.")]
        [Column(TypeName = "datetime")]
        [Display(Name = "Ngày sản xuất")]
        public DateTime? ManufactureDate { get; set; }

        [Required(ErrorMessage = "Xuất xứ là bắt buộc.")]
        [StringLength(100, ErrorMessage = "Xuất xứ không được vượt quá 100 ký tự.")]
        [Display(Name = "Xuất xứ")]
        public string Origin { get; set; }

        [Required(ErrorMessage = "Thời hạn bảo hành là bắt buộc.")]
        [Range(0, int.MaxValue, ErrorMessage = "Thời hạn bảo hành phải lớn hơn hoặc bằng 0.")]
        [Display(Name = "Thời hạn bảo hành (tháng)")]
        public int? WarrantyPeriod { get; set; }

        [Required(ErrorMessage = "Ngày tạo là bắt buộc.")]
        [Column(TypeName = "datetime")]
        [Display(Name = "Ngày tạo")]
        public DateTime? CreatedDate { get; set; }

        [Required(ErrorMessage = "Ngày cập nhật là bắt buộc.")]
        [Column(TypeName = "datetime")]
        [Display(Name = "Ngày cập nhật cuối")]
        public DateTime? LastUpdatedDate { get; set; }

        [Display(Name = "Ngừng kinh doanh")]
        public bool IsDiscontinued { get; set; }

        [ForeignKey("CategoryId")]
        [InverseProperty("Products")]
        public virtual Category Category { get; set; }

        [InverseProperty("Product")]
        public virtual ICollection<Comment> Comments { get; set; } = new List<Comment>();

        [InverseProperty("Product")]
        public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();

        [InverseProperty("Product")]
        public virtual ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();

        [InverseProperty("Product")]
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; } = new List<ShoppingCart>();
    }

}
