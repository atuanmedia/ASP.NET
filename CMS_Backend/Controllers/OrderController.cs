/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
Mssv: 2123110162
Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        // Khai báo biến context để làm việc với Database
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo và Inject DbContext
        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách đơn hàng
        public IActionResult Index()
        {
            // Lấy toàn bộ dữ liệu từ bảng Orders
            var orders = _context.Orders.ToList();

            // Trả dữ liệu sang View
            return View(orders);
        }

        // Hiển thị chi tiết đơn hàng
        public IActionResult Details(int id)
        {
            // Tìm đơn hàng theo Id
            var order = _context.Orders.Find(id);

            // Nếu không tìm thấy thì trả về lỗi 404
            if (order == null)
                return NotFound();

            // Trả dữ liệu sang View
            return View(order);
        }
    }
}