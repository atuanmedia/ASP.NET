/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    // ViewModel truyền dữ liệu thống kê ra View
    public class DashboardViewModel
    {
        public int TotalProducts    { get; set; }
        public int TotalOrders      { get; set; }
        public int TotalCustomers   { get; set; }
        public int TotalPosts       { get; set; }

        public int OrdersPending    { get; set; } // Status = 0
        public int OrdersShipping   { get; set; } // Status = 1
        public int OrdersDone       { get; set; } // Status = 2

        public List<Order>  RecentOrders { get; set; } = new();
        public List<Post>   RecentPosts  { get; set; } = new();
    }

    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var vm = new DashboardViewModel
            {
                TotalProducts  = _context.Products.Count(),
                TotalOrders    = _context.Orders.Count(),
                TotalCustomers = _context.Customers.Count(),
                TotalPosts     = _context.Posts.Count(),

                OrdersPending  = _context.Orders.Count(o => o.Status == 0),
                OrdersShipping = _context.Orders.Count(o => o.Status == 1),
                OrdersDone     = _context.Orders.Count(o => o.Status == 2),

                RecentOrders = _context.Orders
                    .Include(o => o.Customer)
                    .Include(o => o.OrderDetails)
                    .OrderByDescending(o => o.OrderDate)
                    .Take(7)
                    .ToList(),

                RecentPosts = _context.Posts
                    .Include(p => p.Category)
                    .OrderByDescending(p => p.CreatedDate)
                    .Take(5)
                    .ToList()
            };

            return View(vm);
        }
    }
}
