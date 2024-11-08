namespace Sell_​_cleaning_services_e_commerce.Models.ViewModel
{
    public class ProductTemp
    {
        public String ProductName { get; set; }
        public String ProductId { get; set; }

        public String Description { get; set; }
        public decimal Price { get; set; }
        public string img { get; set; }
        public string fullname { get; set; }
        public DateTime? CreatedDate { get; internal set; }
    }
}
