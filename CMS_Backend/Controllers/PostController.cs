/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
Mssv: 2123110162
Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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
        // 1. Hàm hiển thị form tạo mới bài viết (GET)
        [HttpGet]
        public IActionResult Create()
        {
            // Chúng ta lấy danh sách Category để đổ vào ViewBag
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }



        [HttpPost]
        public IActionResult Create(Post model, IFormFile uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // 1. Định nghĩa đường dẫn lưu file: wwwroot/uploads
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

                // Tạo thư mục nếu chưa tồn tại
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                // 2. Tạo tên file duy nhất để không bị đè dữ liệu
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                // 3. Chép file vào thư mục
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // 4. Lưu đường dẫn vào CSDL để sau này hiển thị
                model.ImageUrl = "/uploads/" + fileName;
            }

            _context.Posts.Add(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }
        public IActionResult Delete(int id)
        {
            // 1. Tìm bài viết theo Id
            var post = _context.Posts.Find(id);

            if (post != null)
            {
                // 2. Xóa khỏi bộ nhớ tạm
                _context.Posts.Remove(post);

                // 3. Cập nhật xuống SQL Server
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
        // GET: Hiển thị form kèm dữ liệu cũ
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var post = _context.Posts.Find(id);
            if (post == null) return NotFound();

            // Chuẩn bị lại danh sách danh mục để người dùng có thể đổi chuyên mục
            ViewBag.CategoryList = new SelectList(_context.Categories, "Id", "Name", post.CategoryId);
            return View(post);
        }

        // POST: Thực hiện cập nhật
        [HttpPost]
        public IActionResult Edit(Post model, IFormFile uploadImage)
        {
            // Bước 1: Kiểm tra xem người dùng có chọn file ảnh mới không
            if (uploadImage != null && uploadImage.Length > 0)
            {
                // Thực hiện quy trình upload giống như trang Create
                string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
                if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);

                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(uploadImage.FileName);
                string filePath = Path.Combine(folder, fileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    uploadImage.CopyTo(stream);
                }

                // Cập nhật đường dẫn ảnh mới vào model
                model.ImageUrl = "/uploads/" + fileName;
            }
            else
            {
                // Bước quan trọng: Nếu không upload ảnh mới, chúng ta phải giữ lại ảnh cũ
                // Chúng ta cần lấy lại giá trị ImageUrl từ Database để tránh bị ghi đè thành rỗng
                var oldPost = _context.Posts.AsNoTracking().FirstOrDefault(p => p.Id == model.Id);
                if (oldPost != null && string.IsNullOrEmpty(model.ImageUrl))
                {
                    model.ImageUrl = oldPost.ImageUrl;
                }
            }
            _context.Posts.Update(model);
            _context.SaveChanges();
            return RedirectToAction("Index");
        }

    }
}