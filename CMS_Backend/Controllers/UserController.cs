using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext
        public UserController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách người dùng
        public IActionResult Index()
        {
            // Lấy dữ liệu thật từ Database
            var users = _context.Users.ToList();

            return View(users);
        }

        // Hiển thị chi tiết người dùng
        public IActionResult Details(int id)
        {
            // Tìm người dùng theo Id
            var user = _context.Users.Find(id);

            if (user == null)
                return NotFound();

            return View(user);
        }
    }
}