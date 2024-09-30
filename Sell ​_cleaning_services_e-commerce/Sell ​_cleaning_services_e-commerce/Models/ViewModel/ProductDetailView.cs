using System.ComponentModel.DataAnnotations;

namespace Sell_​_cleaning_services_e_commerce.Models.ViewModel
{
    public class ProductDetailView
    {
        public Product Product { get; set; }

        [Display(Name = "Hình sản phẩm")]
        public List<IFormFile> ProductImages { get; set; }
    }
}
