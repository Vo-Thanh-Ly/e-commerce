using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Data;
using Sell_​_cleaning_services_e_commerce.Models;
using System.Diagnostics;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            //Sản phẩm mới
            var products = _context.Products
            .Include(p => p.ProductImages)
            .ToList();

            var newProducts = products.Select(p => new ProductTemp
            {
                ProductName = p.ProductName.Length > 20 ? p.ProductName.Substring(0, 20) + "..." : p.ProductName,
                fullname = p.ProductName,
                ProductId = p.ProductId.ToString(),
                Description = p.Description.Length > 100 ? p.Description.Substring(0, 100) + "..." : p.Description,
                Price = p.Price,
                img = p.ProductImages.Any()
                    ? p.ProductImages.FirstOrDefault()?.ImageUrl ?? "/image/products/default/default.jpg"
                    : "/image/products/default/default.jpg"
            }).Take(12).ToList();

            // Lưu danh sách newProducts vào ViewBag
            ViewBag.NewProducts = newProducts;


            //Danh mục sản phẩm
            var danhMucs = _context.Categories.ToList();
            ViewBag.DanhMucs = danhMucs;
            List<productForCatelogies> List = new List<productForCatelogies>();
            foreach (Category categorie in danhMucs)
            {
                var product = _context.Products
                            .Where(p => p.CategoryId == categorie.CategoryId)
                            .ToList();
                var Products01 = product.Select(p => new ProductTemp
                {
                    ProductName = p.ProductName.Length > 20 ? p.ProductName.Substring(0, 20) + "..." : p.ProductName,
                    fullname = p.ProductName,
                    ProductId = p.ProductId.ToString(),
                    Description = p.Description.Length > 100 ? p.Description.Substring(0, 100) + "..." : p.Description,
                    Price = p.Price,
                    img = p.ProductImages.Any()
                        ? p.ProductImages.FirstOrDefault()?.ImageUrl ?? "/image/products/default/default.jpg"
                        : "/image/products/default/default.jpg"
                }).Take(12).ToList();
                productForCatelogies productForCatelogies = new productForCatelogies();
                productForCatelogies.Products = Products01;
                List.Add(productForCatelogies);
            }

            ViewBag.List = List;


            //Máy hút bụi

            //Máy đánh sàn

            //Dụng cụ cầm tay

            //Sản phẩm nối bật
            //sản phẩm mới

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }

    class ProductTemp
    {
        public String ProductName { get; set; }
        public String ProductId { get; set; }

        public String Description { get; set; }
        public decimal Price { get; set; }
        public string img { get; set; }
        public string fullname { get; set; }
    }

    class productForCatelogies
    {
        public List<ProductTemp> Products { get; set; } = new List<ProductTemp>();
    }
}
