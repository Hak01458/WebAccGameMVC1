using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAccGameMVC.Models;
using WebAccGameMVC1.Constants;
using WebAccGameMVC1.Data;

namespace WebAccGameMVC1.Controllers
{
    [Authorize(Roles = nameof(Roles.Admin))]
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public AdminController(ApplicationDbContext context, IWebHostEnvironment env)
        {
            _context = context;
            _env = env;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> TopSellingProducts()
        {
            var ranking = await
                (from od in _context.OrderDetails.AsNoTracking()
                 join o in _context.Orders.AsNoTracking() on od.OrderId equals o.OrderId
                 join p in _context.Products.AsNoTracking() on od.ProductId equals p.ProductId
                 where o.Status == "Confirmed" || o.Status == "confirmed"
                 group new { od, o, p } by new { od.ProductId, p.ProductName } into g
                 orderby g.Sum(x => x.od.Quantity) descending, g.Key.ProductName
                 select new AdminTopSellingProductViewModel
                 {
                     ProductId = g.Key.ProductId,
                     ProductName = g.Key.ProductName,
                     SoldQuantity = g.Sum(x => x.od.Quantity),
                     TotalRevenue = g.Sum(x => x.od.Quantity * x.od.Price),
                     ConfirmedOrderCount = g.Select(x => x.o.OrderId).Distinct().Count()
                 })
                .ToListAsync();

            for (var i = 0; i < ranking.Count; i++)
            {
                ranking[i].Rank = i + 1;
            }

            return View(ranking);
        }

        #region Order Management
        [HttpGet]
        public async Task<IActionResult> Orders()
        {
            var orders = await _context.Orders
                .AsNoTracking()
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();

            return View(orders);
        }

        [HttpGet]
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Orders));
            }

            return View(order);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateOrderStatus(int orderId, string status)
        {
            var validStatuses = new[] { "Pending", "Confirmed", "Shipped", "Delivered", "Cancelled" };
            if (!validStatuses.Contains(status))
            {
                TempData["Error"] = "Trạng thái không hợp lệ";
                return RedirectToAction(nameof(Orders));
            }

            var order = await _context.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Orders));
            }

            order.Status = status;
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã cập nhật trạng thái đơn #{order.OrderId}";
            return RedirectToAction(nameof(Orders));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Orders));
            }

            if (order.OrderDetails.Any())
            {
                _context.OrderDetails.RemoveRange(order.OrderDetails);
            }

            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xóa đơn hàng #{orderId}";
            return RedirectToAction(nameof(Orders));
        }

        // Xác nhận đơn hàng (trước đây gọi là "Làm bánh" và có kiểm tra tồn kho nguyên liệu,
        // nhưng vì database hiện tại chưa có bảng Ingredient/Recipe nên chỉ đơn giản là
        // chuyển trạng thái đơn từ Pending -> Confirmed).
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MakeCake(int orderId)
        {
            var order = await _context.Orders
                .Include(o => o.OrderDetails)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);

            if (order == null)
            {
                TempData["Error"] = "Không tìm thấy đơn hàng";
                return RedirectToAction(nameof(Orders));
            }

            if (!order.OrderDetails.Any())
            {
                TempData["Warning"] = "Đơn hàng chưa có sản phẩm nào";
                return RedirectToAction(nameof(OrderDetails), new { orderId });
            }

            if (order.Status != "Pending")
            {
                TempData["Error"] = "Chỉ có thể xác nhận đơn đang ở trạng thái Pending";
                return RedirectToAction(nameof(OrderDetails), new { orderId });
            }

            order.Status = "Confirmed";
            await _context.SaveChangesAsync();

            TempData["Success"] = $"Đã xác nhận đơn #{order.OrderId}";
            return RedirectToAction(nameof(OrderDetails), new { orderId });
        }
        #endregion

        #region Category Management
        [HttpGet]
        public async Task<IActionResult> Categories()
        {
            var categories = await _context.Categories
                .OrderBy(c => c.CategoryId)
                .ToListAsync();

            return View(categories);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCategory(string categoryName)
        {
            if (string.IsNullOrWhiteSpace(categoryName))
            {
                TempData["Error"] = "Tên danh mục không được để trống";
                return RedirectToAction(nameof(Categories));
            }

            var exists = await _context.Categories
                .AnyAsync(c => c.CategoryName == categoryName.Trim());

            if (exists)
            {
                TempData["Error"] = "Danh mục đã tồn tại";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Add(new Category { CategoryName = categoryName.Trim() });
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thêm danh mục thành công";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCategory(int categoryId, string categoryName)
        {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục";
                return RedirectToAction(nameof(Categories));
            }

            if (string.IsNullOrWhiteSpace(categoryName))
            {
                TempData["Error"] = "Tên danh mục không được để trống";
                return RedirectToAction(nameof(Categories));
            }

            category.CategoryName = categoryName.Trim();
            await _context.SaveChangesAsync();

            TempData["Success"] = "Cập nhật danh mục thành công";
            return RedirectToAction(nameof(Categories));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCategory(int categoryId)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);

            if (category == null)
            {
                TempData["Error"] = "Không tìm thấy danh mục";
                return RedirectToAction(nameof(Categories));
            }

            if (category.Products.Any())
            {
                TempData["Error"] = "Không thể xóa danh mục đang có sản phẩm";
                return RedirectToAction(nameof(Categories));
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa danh mục thành công";
            return RedirectToAction(nameof(Categories));
        }
        #endregion

        #region Product Management
        [HttpGet]
        public async Task<IActionResult> Products()
        {
            var products = await _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .OrderBy(p => p.ProductName)
                .ToListAsync();

            ViewBag.Categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return View(products);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateProduct(
            string productName,
            decimal price,
            int categoryId,
            string? description,
            IFormFile? imageFile)
        {
            if (string.IsNullOrWhiteSpace(productName))
            {
                TempData["Error"] = "Tên bánh không được để trống";
                return RedirectToAction(nameof(Products));
            }

            if (price < 0)
            {
                TempData["Error"] = "Giá phải >= 0";
                return RedirectToAction(nameof(Products));
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
            if (!categoryExists)
            {
                TempData["Error"] = "Danh mục không hợp lệ";
                return RedirectToAction(nameof(Products));
            }

            var imagePath = await SaveProductImageAsync(imageFile);

            _context.Products.Add(new Product
            {
                ProductName = productName.Trim(),
                Price = price,
                CategoryId = categoryId,
                Description = description?.Trim(),
                Image = imagePath
            });

            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm bánh thành công";
            return RedirectToAction(nameof(Products));
        }

        [HttpGet]
        public async Task<IActionResult> EditProduct(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy bánh";
                return RedirectToAction(nameof(Products));
            }

            ViewBag.Categories = await _context.Categories
                .AsNoTracking()
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditProduct(
            int productId,
            string productName,
            decimal price,
            int categoryId,
            string? description,
            IFormFile? imageFile)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy bánh";
                return RedirectToAction(nameof(Products));
            }

            if (string.IsNullOrWhiteSpace(productName))
            {
                TempData["Error"] = "Tên bánh không được để trống";
                return RedirectToAction(nameof(EditProduct), new { productId });
            }

            if (price < 0)
            {
                TempData["Error"] = "Giá phải >= 0";
                return RedirectToAction(nameof(EditProduct), new { productId });
            }

            var categoryExists = await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
            if (!categoryExists)
            {
                TempData["Error"] = "Danh mục không hợp lệ";
                return RedirectToAction(nameof(EditProduct), new { productId });
            }

            if (imageFile != null && imageFile.Length > 0)
            {
                product.Image = await SaveProductImageAsync(imageFile);
            }

            product.ProductName = productName.Trim();
            product.Price = price;
            product.CategoryId = categoryId;
            product.Description = description?.Trim();

            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật bánh thành công";
            return RedirectToAction(nameof(Products));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteProduct(int productId)
        {
            var product = await _context.Products.FirstOrDefaultAsync(p => p.ProductId == productId);
            if (product == null)
            {
                TempData["Error"] = "Không tìm thấy bánh";
                return RedirectToAction(nameof(Products));
            }

            // chặn xóa nếu đã có trong đơn hàng
            var usedInOrders = await _context.OrderDetails.AnyAsync(od => od.ProductId == productId);
            if (usedInOrders)
            {
                TempData["Error"] = "Không thể xóa bánh vì đã tồn tại trong đơn hàng";
                return RedirectToAction(nameof(Products));
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Xóa bánh thành công";
            return RedirectToAction(nameof(Products));
        }

        private async Task<string?> SaveProductImageAsync(IFormFile? imageFile)
        {
            if (imageFile == null || imageFile.Length == 0) return null;

            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads", "products");
            Directory.CreateDirectory(uploadsFolder);

            var ext = Path.GetExtension(imageFile.FileName);
            var fileName = $"{Guid.NewGuid():N}{ext}";
            var fullPath = Path.Combine(uploadsFolder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await imageFile.CopyToAsync(stream);

            return $"/uploads/products/{fileName}";
        }

        #endregion
    }
}
