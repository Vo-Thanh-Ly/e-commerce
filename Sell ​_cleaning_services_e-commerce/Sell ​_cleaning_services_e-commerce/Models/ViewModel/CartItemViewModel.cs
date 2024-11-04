namespace Sell_​_cleaning_services_e_commerce.Models.ViewModel
{
    public class CartItemViewModel
    {
        public int ProductId { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public double Price { get; set; }
        public double TotalPrice => Quantity * Price; // Tính tổng tiền
        public int MaxQuantity { get; set; }
    }

}
