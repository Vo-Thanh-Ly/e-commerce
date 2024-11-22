using Microsoft.AspNetCore.Identity;

namespace Sell_​_cleaning_services_e_commerce.Models
{
    public class User : IdentityUser
    {
        public bool? Sex { get; set; }           // Nullable boolean
        public DateOnly? DateOfBirth { get; set; } // Nullable DateOnly
        public string? Avatar { get; set; }       // Nullable string
        // Các thuộc tính điều hướng đến các đối tượng khác
        public virtual ICollection<Comment> Comments { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; }
        public virtual ICollection<ShoppingCart> ShoppingCarts { get; set; }
        public string? Address { get; set; }
        public string? Nomalname { get;set; }
    }
}
