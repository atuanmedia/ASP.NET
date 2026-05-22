using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class CustomerController : Controller
    {
        // Khai báo biến context để làm việc với Database
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo và Inject DbContext
        public CustomerController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách khách hàng
        public IActionResult Index()
        {
            // Lấy toàn bộ dữ liệu từ bảng Customers
            var customers = _context.Customers.ToList();

            // Trả dữ liệu sang View
            return View(customers);
        }

        // Hiển thị chi tiết khách hàng
        public IActionResult Details(int id)
        {
            // Tìm khách hàng theo Id
            var customer = _context.Customers.Find(id);

            // Nếu không tìm thấy thì trả về lỗi 404
            if (customer == null)
                return NotFound();

            // Trả dữ liệu sang View
            return View(customer);
        }
    }
}