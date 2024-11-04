using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Areas.Other;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Models;
using Sell__cleaning_services_e_commerce.Models.ViewModel;
using X.PagedList;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{

    public class ShopController : Controller
    {
        private readonly ApplicationDbContext _context;
        private UserManager<User> _userManager;
        public ShopController(ApplicationDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Shop
        public async Task<IActionResult> Index(int? page, string sortOrder, int? categoriesId, string SearchString)
        {
            ViewBag.SearchString = SearchString;

            // Phân trang
            int pageSize = 15;
            int pageNumber = page ?? 1;
            var products = await _context.Products
                             .Include(p => p.ProductImages)
                             .ToListAsync();

            // Lấy danh sách các categoriesName
            ViewBag.CategoryId = categoriesId;
            ViewBag.Category = _context.Categories.Select(ca => new Temp
            {
                CategoryId = ca.CategoryId,
                CategoryName = ca.CategoryName
            }).ToList();

            var categoriesNames = _context.Categories.Select(c => c.CategoryName);

            // Lọc theo Categories
            if (categoriesId != null)
            {
                products = await _context.Products
                             .Include(p => p.ProductImages).Where(p => p.CategoryId == categoriesId)
                             .ToListAsync();
            }

            // Sản phẩm hiển thị và sắp xếp sản phẩm
            sortOrder = string.IsNullOrEmpty(sortOrder) ? "MoiNhat" : sortOrder;
            ViewBag.CurrentSort = sortOrder;

            // Khai báo biến newProducts bên ngoài khối if-else
            IEnumerable<ProductTemp> newProducts;

            // Lấy danh sách sản phẩm
            if (!String.IsNullOrEmpty(SearchString))
            {

                newProducts = products.Select(p => new ProductTemp
                {
                    ProductName = p.ProductName.Length > 15 ? p.ProductName.Substring(0, 15) + "..." : p.ProductName,
                    fullname = p.ProductName,
                    ProductId = p.ProductId.ToString(),
                    Description = p.Description.Length > 40 ? p.Description.Substring(0, 40) + "..." : p.Description,
                    Price = (int)p.Price,
                    CreatedDate = p.CreatedDate,
                    img = p.ProductImages.Any() ? p.ProductImages.FirstOrDefault()?.ImageUrl ?? "/image/products/default/default.jpg" : "/image/products/default/default.jpg"
                }).Where(p => p.ProductName.ToLower().Contains(SearchString.ToLower()));
            }
            else
            {
                newProducts = products.Select(p => new ProductTemp
                {
                    ProductName = p.ProductName.Length > 15 ? p.ProductName.Substring(0, 15) + "..." : p.ProductName,
                    fullname = p.ProductName,
                    ProductId = p.ProductId.ToString(),
                    Description = p.Description.Length > 40 ? p.Description.Substring(0, 40) + "..." : p.Description,
                    Price = (int)p.Price,
                    CreatedDate = p.CreatedDate,
                    img = p.ProductImages.Any() ? p.ProductImages.FirstOrDefault()?.ImageUrl ?? "/image/products/default/default.jpg" : "/image/products/default/default.jpg"
                });
            }

            // Sắp xếp theo sortOrder
            switch (sortOrder)
            {
                case "MoiNhat":
                    newProducts = newProducts.OrderByDescending(p => p.CreatedDate);
                    break;
                case "TangDan":
                    newProducts = newProducts.OrderBy(p => p.Price);
                    break;
                case "GiamDan":
                    newProducts = newProducts.OrderByDescending(p => p.Price);
                    break;
                default:
                    newProducts = newProducts.OrderByDescending(p => p.CreatedDate);
                    break;
            }

            // Phân trang
            var pagedProducts = newProducts.ToPagedList(pageNumber, pageSize);

            return View(pagedProducts);
        }


        public async Task<IActionResult> ProductDetail(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            //Lấy tất cả các ảnh của sản phẩm
            var images = await _context.ProductImages
                                   .Where(i => i.ProductId == id)
                                   .ToListAsync();
            if (images != null)
            {
                ViewBag.Images = images;
            }

            var comments = await _context.Comments
                                     .Where(c => c.ProductId == id)
                                     .ToListAsync();
            //Lấy tất cả comment
            List<CommentDetail> commentsDetails = new List<CommentDetail>();
            foreach (var comment in comments)
            {
                var user01 = await _context.Users.FindAsync(comment.UserId);

                CommentDetail commentsDetail = new CommentDetail();
                commentsDetail.UserName = user01.UserName;
                commentsDetail.ProductId = comment.ProductId;
                commentsDetail.UserId = comment.UserId;
                commentsDetail.Content = comment.Content;
                commentsDetail.CreatedDate = comment.CreatedDate;
                commentsDetail.Rating = comment.Rating;
                commentsDetails.Add(commentsDetail);
            }
            if (comments != null)
            {
                ViewBag.Comments = commentsDetails;
                // Tính toán giá trị trung bình của đánh giá
                ViewBag.AverageRating = comments.Any() ? comments.Average(c => c.Rating ?? 0) : 0;
            }
            return View(product);
        }


        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public class CommentDetail
        {
            public string UserName { get; set; }
            public int ProductId { get; set; }
            public string UserId { get; set; }
            public string Content { get; set; }
            public DateTime? CreatedDate { get; set; }
            public int? Rating { get; set; }
        }


        [HttpPost]
        [Authorize(Roles = RoleList.Customer)]
        public async Task<IActionResult> PostComment(int productId, string content, int rating)
        {
            if (string.IsNullOrEmpty(content))
            {
                return BadRequest("Nội dung bình luận không được để trống.");
            }

            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return Challenge(); // Chuyển hướng đến trang đăng nhập nếu người dùng chưa đăng nhập
            }

            var comment = new Comment
            {
                ProductId = productId,
                UserId = user.Id,
                Content = content,
                Rating = rating,
                CreatedDate = DateTime.Now
            };

            _context.Comments.Add(comment);
            await _context.SaveChangesAsync();

            return RedirectToAction("ProductDetail", new { id = productId });
        }

    }

    public class Temp
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

    }
}
