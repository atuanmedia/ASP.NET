using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class ProductController : Controller
    {
        // Khai báo biến context để làm việc với Database
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo và Inject DbContext
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách sản phẩm
        public IActionResult Index()
        {
            // Lấy toàn bộ dữ liệu từ bảng Products
            var products = _context.Products.ToList();

            // Trả dữ liệu sang View
            return View(products);
        }

        // Hiển thị chi tiết sản phẩm theo Id
        public IActionResult Details(int id)
        {
            // Tìm sản phẩm theo Id
            var product = _context.Products.Find(id);

            // Nếu không tìm thấy thì trả về lỗi 404
            if (product == null)
                return NotFound();

            // Trả dữ liệu sang View
            return View(product);
        }
    }
}