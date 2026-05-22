using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CMS.Backend.Controllers
{
    public class HomeController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext vào Controller
        public HomeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị 3 bản tin mới nhất lên trang chủ
        public IActionResult Index()
        {
            // LINQ: Kết hợp Include, OrderByDescending và Take để lấy đúng 3 bài mới nhất
            var latestPosts = _context.Posts
                                      .Include(p => p.Category) // Lấy kèm tên danh mục để hiển thị ở giao diện
                                      .OrderByDescending(p => p.CreatedDate) // Sắp xếp ngày mới nhất lên đầu
                                      .Take(3) // Chỉ lấy đúng 3 bản ghi đầu tiên
                                      .ToList();

            // Truyền danh sách 3 bài viết ra View trang chủ
            return View(latestPosts);
        }
    }
}