/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesProductController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext vào Controller
        public CategoriesProductController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =====================================
        // GET ALL CATEGORIES
        // URL: GET api/categoriesproduct
        // Mục đích: Lấy danh sách toàn bộ danh mục hiển thị lên Menu/Sidebar ở Client
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var categories = await _context.CategoriesProducts
                                           .Select(c => new
                                           {
                                               c.Id,
                                               c.Name,
                                               c.Description,
                                               c.ImageUrl
                                           })
                                           .ToListAsync();

            return Ok(categories);
        }

        // =====================================
        // GET CATEGORY BY ID WITH PRODUCTS (Lọc sản phẩm theo danh mục)
        // URL: GET api/categoriesproduct/5
        // Mục đích: Khi Client click vào 1 danh mục cụ thể -> Trả về thông tin danh mục đó + danh sách sản phẩm bên trong
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // Sử dụng chính xác quan hệ dữ liệu dựa trên thuộc tính c.Products và khóa ngoại p.CategoryProductId
            var category = await _context.CategoriesProducts
                                         .Where(c => c.Id == id)
                                         .Select(c => new
                                         {
                                             c.Id,
                                             c.Name,
                                             c.Description,
                                             c.ImageUrl,
                                             // Lọc toàn bộ danh sách sản phẩm thuộc về danh mục này dựa theo CategoryProductId
                                             Products = _context.Products
                                                                .Where(p => p.CategoryProductId == c.Id)
                                                                .Select(p => new
                                                                {
                                                                    p.Id,
                                                                    p.Name,
                                                                    p.Price,
                                                                    p.StockQuantity,
                                                                    p.ImageUrl,
                                                                    p.Description
                                                                }).ToList()
                                         })
                                         .FirstOrDefaultAsync();

            // Nếu không tìm thấy danh mục yêu cầu
            if (category == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục sản phẩm có Id = {id}" });
            }

            return Ok(category);
        }
    }
}