/*
 High And Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace CMS_Backend.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public PostsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Tạo 1 lớp DTO dùng riêng cho việc Nhận dữ liệu từ JSON gửi lên (Tránh lỗi 400)
        public class PostInputDto
        {
            public string Title { get; set; }
            public string Content { get; set; }
            public string ImageUrl { get; set; }
            public int CategoryId { get; set; }
        }

        // =====================================
        // GET ALL POSTS
        // =====================================
        [HttpGet]
        public IActionResult GetAll()
        {
            var posts = _context.Posts
                                .Include(p => p.Category)
                                .OrderByDescending(p => p.CreatedDate)
                                .Select(p => new
                                {
                                    p.Id,
                                    p.Title,
                                    p.Content,
                                    p.ImageUrl,
                                    p.CreatedDate,
                                    p.CategoryId,
                                    CategoryName = p.Category != null ? p.Category.Name : "" // Tránh lỗi null nếu Category trống
                                })
                                .ToList();

            return Ok(posts);
        }

        // =====================================
        // GET POST BY ID
        // =====================================
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var post = _context.Posts
                               .Include(p => p.Category)
                               .Where(p => p.Id == id)
                               .Select(p => new
                               {
                                   p.Id,
                                   p.Title,
                                   p.Content,
                                   p.ImageUrl,
                                   p.CreatedDate,
                                   Category = p.Category != null ? new
                                   {
                                       p.Category.Id,
                                       p.Category.Name
                                   } : null
                               })
                               .FirstOrDefault();

            if (post == null)
            {
                return NotFound(new { message = "Không tìm thấy bài viết" });
            }

            return Ok(post);
        }

        // =====================================
        // CREATE POST
        // =====================================
        [HttpPost]
        public IActionResult Create([FromBody] PostInputDto input) // Đổi sang PostInputDto để an toàn
        {
            if (input == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra CategoryId xem có tồn tại thật trong DB không
            var categoryExists = _context.Categories.Any(c => c.Id == input.CategoryId);
            if (!categoryExists)
            {
                return BadRequest(new { message = "Danh mục (CategoryId) không tồn tại!" });
            }

            // Chuyển đổi dữ liệu từ DTO sang Entity thực tế để lưu vào DB
            var newPost = new Post
            {
                Title = input.Title,
                Content = input.Content,
                ImageUrl = input.ImageUrl,
                CategoryId = input.CategoryId,
                CreatedDate = DateTime.Now
            };

            _context.Posts.Add(newPost);
            _context.SaveChanges();

            return Ok(new { message = "Thêm bài viết thành công", data = newPost });
        }

        // =====================================
        // UPDATE POST
        // =====================================
        [HttpPut("{id}")]
        public IActionResult Update(int id, [FromBody] PostInputDto input) // Thêm [FromBody] và DTO ở đây
        {
            var post = _context.Posts.Find(id);

            if (post == null)
            {
                return NotFound(new { message = "Không tìm thấy bài viết" });
            }

            if (input == null || !ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Cập nhật dữ liệu từ DTO đầu vào
            post.Title = input.Title;
            post.Content = input.Content;
            post.ImageUrl = input.ImageUrl;
            post.CategoryId = input.CategoryId;

            _context.SaveChanges();

            return Ok(new
            {
                message = "Cập nhật bài viết thành công",
                data = post
            });
        }

        // =====================================
        // DELETE POST
        // =====================================
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var post = _context.Posts.Find(id);

            if (post == null)
            {
                return NotFound(new { message = "Không tìm thấy bài viết" });
            }

            _context.Posts.Remove(post);
            _context.SaveChanges();

            return Ok(new { message = "Xóa bài viết thành công" });
        }
    }
}