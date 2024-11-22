using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Data;
using Sell_​_cleaning_services_e_commerce.Models;
using Sell__cleaning_services_e_commerce.Models.ViewModel;
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
                ProductName = p.ProductName.Length > 15 ? p.ProductName.Substring(0, 15) + "..." : p.ProductName,
                fullname = p.ProductName,
                ProductId = p.ProductId.ToString(),
                Description = p.Description.Length > 40 ? p.Description.Substring(0, 40) + "..." : p.Description,
                Price = (int)p.Price,
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
                    ProductName = p.ProductName.Length > 15 ? p.ProductName.Substring(0, 15) + "..." : p.ProductName,
                    fullname = p.ProductName,
                    ProductId = p.ProductId.ToString(),
                    Description = p.Description.Length > 40 ? p.Description.Substring(0, 40) + "..." : p.Description,
                    Price = (int)p.Price,
                    img = p.ProductImages.Any()
                        ? p.ProductImages.FirstOrDefault()?.ImageUrl ?? "/image/products/default/default.jpg"
                        : "/image/products/default/default.jpg"
                }).Take(12).ToList();
                productForCatelogies productForCatelogies = new productForCatelogies();
                productForCatelogies.Products = Products01;
                List.Add(productForCatelogies);
            }

            ViewBag.List = List;


            //Sản phẩm nối bật
            // Lấy 10 sản phẩm có số lượng bán ra cao nhất
            var topProducts = _context.InvoiceDetails
                .GroupBy(id => id.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantitySold = g.Sum(id => id.Quantity)
                })
                .OrderByDescending(p => p.TotalQuantitySold)
                .Take(10)
                .ToList();

            // Lấy thông tin chi tiết sản phẩm từ bảng Product
            var topProductDetails = topProducts
                .Join(_context.Products,
                      tp => tp.ProductId,
                      p => p.ProductId,
                      (tp, p) => new
                      {
                          p.ProductId,
                          ProductName = p.ProductName.Length > 15 ? p.ProductName.Substring(0, 15) + "..." : p.ProductName,
                          Description = p.Description.Length > 40 ? p.Description.Substring(0, 40) + "..." : p.Description,
                          Price = (int)p.Price,
                          TotalSold = tp.TotalQuantitySold,
                          ImageUrl = p.ProductImages.Any()
                                     ? p.ProductImages.FirstOrDefault().ImageUrl
                                     : "/image/products/default/default.jpg"
                      })
                .ToList();

            // Truyền danh sách sản phẩm vào ViewBag để hiển thị ra view
            ViewBag.TopProducts = topProductDetails;


            return View();
        }

       

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }


    class productForCatelogies
    {
        public List<ProductTemp> Products { get; set; } = new List<ProductTemp>();
    }

}
