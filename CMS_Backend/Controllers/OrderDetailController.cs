using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class OrderDetailController : Controller
    {
        // Khai báo biến context để làm việc với Database
        private readonly ApplicationDbContext _context;

        // Hàm khởi tạo và Inject DbContext
        public OrderDetailController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách chi tiết đơn hàng
        public IActionResult Index()
        {
            // Lấy toàn bộ dữ liệu từ bảng OrderDetails
            var orderDetails = _context.OrderDetails.ToList();

            // Trả dữ liệu sang View
            return View(orderDetails);
        }

        // Hiển thị chi tiết OrderDetail
        public IActionResult Details(int id)
        {
            // Tìm chi tiết đơn hàng theo Id
            var orderDetail = _context.OrderDetails.Find(id);

            // Nếu không tìm thấy thì trả về lỗi 404
            if (orderDetail == null)
                return NotFound();

            // Trả dữ liệu sang View
            return View(orderDetail);
        }
    }
}