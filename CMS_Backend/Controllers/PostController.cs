using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext
        public PostController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách bài viết
        public IActionResult Index()
        {
            // Lấy dữ liệu thật từ Database
            var posts = _context.Posts.ToList();

            return View(posts);
        }

        // Hiển thị chi tiết bài viết
        public IActionResult Details(int id)
        {
            // Tìm bài viết theo Id
            var post = _context.Posts.Find(id);

            if (post == null)
                return NotFound();

            return View(post);
        }
    }
}