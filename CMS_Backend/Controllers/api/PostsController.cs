/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;

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

        // Bỏ tag HTML, lấy đoạn trích ≤160 ký tự
        private static string MakeExcerpt(string? html, int maxLen = 160)
        {
            if (string.IsNullOrWhiteSpace(html)) return "";
            var text = Regex.Replace(html, "<[^>]+>", " ");
            text = Regex.Replace(text, @"\s+", " ").Trim();
            return text.Length > maxLen ? text[..maxLen].TrimEnd() + "…" : text;
        }

        // GET /api/Posts?page=1&pageSize=9&categoryId=
        [HttpGet]
        public async Task<IActionResult> GetAll(
            [FromQuery] int page     = 1,
            [FromQuery] int pageSize = 9,
            [FromQuery] int? categoryId = null)
        {
            var query = _context.Posts
                .AsNoTracking()
                .OrderByDescending(p => p.CreatedDate)
                .AsQueryable();

            if (categoryId.HasValue && categoryId > 0)
                query = query.Where(p => p.CategoryId == categoryId.Value);

            var total = await query.CountAsync();

            // Load từ DB — lấy Content để tạo excerpt trong memory
            var rows = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.ImageUrl,
                    p.CreatedDate,
                    p.CategoryId,
                    p.Content,
                    CategoryName = p.Category != null ? p.Category.Name : "Tin tức"
                })
                .ToListAsync();

            // Tạo excerpt trong memory (không phụ thuộc EF translation)
            var data = rows.Select(p => new
            {
                p.Id,
                p.Title,
                p.ImageUrl,
                p.CreatedDate,
                p.CategoryId,
                p.CategoryName,
                ShortDescription = MakeExcerpt(p.Content)
            });

            return Ok(new
            {
                data,
                total,
                page,
                pageSize,
                totalPages = (int)Math.Ceiling((double)total / pageSize)
            });
        }

        // GET /api/Posts/{id}
        [HttpGet("{id}")]
        public IActionResult GetById(int id)
        {
            var post = _context.Posts
                .AsNoTracking()
                .Where(p => p.Id == id)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.Content,
                    p.ImageUrl,
                    p.CreatedDate,
                    p.CategoryId,
                    CategoryName = p.Category != null ? p.Category.Name : "Tin tức"
                })
                .FirstOrDefault();

            if (post == null)
                return NotFound(new { message = "Không tìm thấy bài viết" });

            var related = _context.Posts
                .AsNoTracking()
                .Where(p => p.CategoryId == post.CategoryId && p.Id != id)
                .OrderByDescending(p => p.CreatedDate)
                .Take(4)
                .Select(p => new
                {
                    p.Id,
                    p.Title,
                    p.ImageUrl,
                    p.CreatedDate,
                    CategoryName = p.Category != null ? p.Category.Name : "Tin tức"
                })
                .ToList();

            return Ok(new
            {
                post.Id,
                post.Title,
                post.Content,
                post.ImageUrl,
                post.CreatedDate,
                post.CategoryName,
                RelatedPosts = related
            });
        }
    }
}
