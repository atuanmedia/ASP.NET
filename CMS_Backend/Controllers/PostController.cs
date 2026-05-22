using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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

        // 1. SỬA LỖI TẠI ĐÂY: Hiển thị danh sách bài viết mới nhất
        public IActionResult Index()
        {
            // BẮT BUỘC phải có .Include(p => p.Category) để View gọi được @item.Category.Name
            var posts = _context.Posts
                                .Include(p => p.Category)
                                .OrderByDescending(p => p.CreatedDate) // Sắp xếp bài mới lên đầu
                                .ToList();

            return View(posts);
        }

        // Lọc bài viết theo danh mục (Hàm này bạn viết đã chuẩn)
        public IActionResult ListByCategory(int? id)
        {
            // Kiểm tra nếu không có id truyền vào thì trả về thông báo lỗi
            if (id == null)
            {
                return BadRequest("Vui lòng cung cấp mã danh mục.");
            }

            // Sử dụng LINQ với tham số 'id' linh hoạt
            var posts = _context.Posts
                                .Where(p => p.CategoryId == id)
                                .OrderByDescending(p => p.CreatedDate)
                                .Include(p => p.Category)
                                .ToList();

            // Lưu ý: View này dùng chung cấu trúc giao diện danh sách bạn gửi hoàn toàn hợp lệ
            return View(posts);
        }

        // 2. CẢI TIẾN TẠI ĐÂY: Hiển thị chi tiết bài viết
        public IActionResult Details(int id)
        {
            // Thay vì dùng .Find(id), dùng FirstOrDefault kèm .Include để nạp luôn Category nếu cần dùng ở trang chi tiết
            var post = _context.Posts
                               .Include(p => p.Category)
                               .FirstOrDefault(p => p.Id == id);

            if (post == null)
                return NotFound();

            return View(post);
        }
    }
}