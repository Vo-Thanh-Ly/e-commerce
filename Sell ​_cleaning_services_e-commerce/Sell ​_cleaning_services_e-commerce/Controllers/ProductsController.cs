using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Sell__cleaning_services_e_commerce.Areas.Other;
using Sell__cleaning_services_e_commerce.Data;
using Sell__cleaning_services_e_commerce.Helpers;
using Sell__cleaning_services_e_commerce.Models;
using Sell__cleaning_services_e_commerce.Models.ViewModel;
using X.PagedList;

namespace Sell_​_cleaning_services_e_commerce.Controllers
{
  //  [Authorize(Roles = RoleList.Admin)]
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private IConfiguration _configuration;
        private IWebHostEnvironment _webHostEnvironment;

        public ProductsController(ApplicationDbContext context, IConfiguration configuration, IWebHostEnvironment webHostEnvironment)
        {
            _configuration = configuration;
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        // GET: Products
        public async Task<IActionResult> Index(int? page)
        {

            int pageSize = 10; // Số lượng item trên mỗi trang
            int pageNumber = page ?? 1; // Trang hiện tại, mặc định là trang 1
            var items = _context.Products.Include(p => p.Category).ToPagedList(pageNumber, pageSize);
            return View(items);
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
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

            return View(product);
        }


        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");

            var productViewModel = new ProductViewModel
            {
                Product = new Product
                {
                    CreatedDate = DateTime.Now,
                    LastUpdatedDate = DateTime.Now
                }
            };

            return View(productViewModel);
        }



        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel productViewModel)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(productViewModel.Product);
                    await _context.SaveChangesAsync();

                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products");
                    var fileUploadHelper = new FileUploadHelper(_configuration);

                    if (productViewModel.ProductImages != null && productViewModel.ProductImages.Count > 0)
                    {
                        foreach (var file in productViewModel.ProductImages)
                        {
                            var fileName = await fileUploadHelper.UploadFile(file, uploadPath);
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                var productImage = new ProductImage
                                {
                                    ProductId = productViewModel.Product.ProductId,
                                    ImageUrl = "/image/products/" + fileName
                                };
                                _context.ProductImages.Add(productImage);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Error occurred while saving: " + ex.Message);
                }
            }

            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", productViewModel.Product.CategoryId);
            return View(productViewModel);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["Images"] = _context.ProductImages.Where(x => x.ProductId == id).ToList();
            return View(product);
        }

        // POST: Products/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ProductId,ProductName,CategoryId,Description,Price,QuantityInStock,Brand,Supplier,Weight,Dimensions,Material,Power,PowerSource,ManufactureDate,Origin,WarrantyPeriod,CreatedDate,LastUpdatedDate,IsDiscontinued")] Product product, List<int> imagesToDelete)
        {
            if (id != product.ProductId)
            {
                return NotFound();
            }

            product.LastUpdatedDate = DateTime.Now;

            if (!ModelState.IsValid)
            {

                try
                {
                    _context.Update(product);

                    // Xóa ảnh cũ nếu được chọn
                    if (imagesToDelete != null && imagesToDelete.Any())
                    {
                        var imagesToRemove = await _context.ProductImages
                            .Where(pi => pi.ProductId == product.ProductId && imagesToDelete.Contains(pi.ImageId))
                            .ToListAsync();

                        foreach (var image in imagesToRemove)
                        {
                            // Xóa file ảnh từ thư mục
                            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", image.ImageUrl.TrimStart('/'));
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }

                            _context.ProductImages.Remove(image);
                        }
                    }

                    await _context.SaveChangesAsync();

                    // Xử lý upload ảnh mới (nếu có)
                    var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "image", "products");
                    var fileUploadHelper = new FileUploadHelper(_configuration);

                    if (Request.Form.Files.Count > 0)
                    {
                        foreach (var file in Request.Form.Files)
                        {
                            var fileName = await fileUploadHelper.UploadFile(file, uploadPath);
                            if (!string.IsNullOrEmpty(fileName))
                            {
                                var productImage = new ProductImage
                                {
                                    ProductId = product.ProductId,
                                    ImageUrl = "/image/products/" + fileName
                                };
                                _context.ProductImages.Add(productImage);
                            }
                        }
                        await _context.SaveChangesAsync();
                    }

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.ProductId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            else
            {
                // Nếu ModelState không hợp lệ, in lỗi ra console
                var errors = ModelState.Values.SelectMany(v => v.Errors);
                foreach (var error in errors)
                {
                    Console.WriteLine("Error: " + error.ErrorMessage);
                }
            }

            // Trả lại View với các dữ liệu cần thiết nếu ModelState không hợp lệ
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", product.CategoryId);
            return View(product);
        }
        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product != null)
            {
                var imgs = await _context.ProductImages.Where(x => x.ProductId == id).ToListAsync();
                foreach (var img in imgs)
                {
                    DeleteImage(img.ImageUrl);
                    _context.ProductImages.Remove(img);
                }
                _context.Products.Remove(product);

            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }

        public void DeleteImage(string imageUrl)
        {
            // Đường dẫn tuyệt đối đến file ảnh trong hệ thống tệp
            var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, imageUrl.TrimStart('~', '/'));

            // Kiểm tra nếu file tồn tại
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);  // Xóa file
            }
        }

    }
}
