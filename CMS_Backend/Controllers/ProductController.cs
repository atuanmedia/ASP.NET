/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
Mssv: 2123110162
Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        // Khai báo biến context để làm việc với Database
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo và Inject DbContext
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================
        // INDEX
        // =========================

        // Hiển thị danh sách sản phẩm
        public IActionResult Index()
        {
            // Include để lấy luôn tên danh mục
            var products = _context.Products
                                   .Include(p => p.CategoryProduct)
                                   .ToList();

            return View(products);
        }

        // =========================
        // DETAILS
        // =========================

        // Hiển thị chi tiết sản phẩm theo Id
        public IActionResult Details(int id)
        {
            var product = _context.Products
                                  .Include(p => p.CategoryProduct)
                                  .FirstOrDefault(p => p.Id == id);

            // Nếu không tìm thấy thì trả về lỗi 404
            if (product == null)
                return NotFound();

            return View(product);
        }

        // =========================
        // CREATE
        // =========================

        // GET: Hiển thị form thêm sản phẩm
        [HttpGet]
        public IActionResult Create()
        {
            // Đổ dữ liệu category ra dropdown
            ViewBag.CategoryList = new SelectList(
                _context.CategoriesProducts,
                "Id",
                "Name"
            );

            return View();
        }

        // POST: Thêm sản phẩm
        [HttpPost]
        public IActionResult Create(Product model, IFormFile uploadImage)
        {
            // Upload ảnh
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // Tạo thư mục uploads nếu chưa có
                string folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                // Tạo tên file mới
                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(uploadImage.FileName);

                string filePath = Path.Combine(folder, fileName);

                // Lưu file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // Lưu đường dẫn vào DB
                model.ImageUrl = "/uploads/" + fileName;
            }

            // Lưu DB
            _context.Products.Add(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // EDIT
        // =========================

        // GET: Hiển thị form sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _context.Products.Find(id);

            if (product == null)
                return NotFound();

            // Đổ category ra dropdown
            ViewBag.CategoryList = new SelectList(
                _context.CategoriesProducts,
                "Id",
                "Name",
                product.CategoryProductId
            );

            return View(product);
        }

        // POST: Cập nhật sản phẩm
        [HttpPost]
        public IActionResult Edit(Product model, IFormFile uploadImage)
        {
            // Nếu có upload ảnh mới
            if (uploadImage != null && uploadImage.Length > 0)
            {
                string folder = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    "uploads"
                );

                if (!Directory.Exists(folder))
                    Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString()
                                  + Path.GetExtension(uploadImage.FileName);

                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                model.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                // Giữ ảnh cũ nếu không upload mới
                var oldProduct = _context.Products
                                         .AsNoTracking()
                                         .FirstOrDefault(p => p.Id == model.Id);

                if (oldProduct != null)
                {
                    model.ImageUrl = oldProduct.ImageUrl;
                }
            }

            // Update DB
            _context.Products.Update(model);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }

        // =========================
        // DELETE
        // =========================

        public IActionResult Delete(int id)
        {
            var product = _context.Products.Find(id);

            if (product != null)
            {
                _context.Products.Remove(product);
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }
    }
}