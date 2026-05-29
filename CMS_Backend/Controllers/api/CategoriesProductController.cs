/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
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

        #region --- DATA TRANSFER OBJECTS (DTOs) ---

        // DTO hứng dữ liệu JSON khi Thêm/Sửa danh mục (Giúp loại bỏ hoàn toàn lỗi 400 Validation)
        public class CategoryProductInputDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
        }

        #endregion

        // =====================================
        // GET ALL CATEGORIES
        // URL: GET api/categoriesproduct
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // 🌟 Đã sửa thành _context.CategoriesProducts theo đúng DbContext của bạn
            var categories = await _context.CategoriesProducts
                                           .Select(c => new
                                           {
                                               c.Id,
                                               c.Name,
                                               c.Description
                                           })
                                           .ToListAsync();

            return Ok(categories);
        }

        // =====================================
        // GET CATEGORY BY ID
        // URL: GET api/categoriesproduct/5
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            // 🌟 Đã sửa thành _context.CategoriesProducts
            var category = await _context.CategoriesProducts
                                         .Where(c => c.Id == id)
                                         .Select(c => new
                                         {
                                             c.Id,
                                             c.Name,
                                             c.Description
                                         })
                                         .FirstOrDefaultAsync();

            if (category == null)
            {
                return NotFound(new { message = $"Không tìm thấy danh mục sản phẩm có Id = {id}" });
            }

            return Ok(category);
        }

        // =====================================
        // CREATE CATEGORY
        // URL: POST api/categoriesproduct
        // =====================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryProductInputDto input)
        {
            if (input == null || string.IsNullOrWhiteSpace(input.Name))
            {
                return BadRequest(new { message = "Tên danh mục sản phẩm không được để trống." });
            }

            // 🌟 Đã sửa thành _context.CategoriesProducts để check trùng tên
            var isDuplicate = await _context.CategoriesProducts.AnyAsync(c => c.Name.ToLower() == input.Name.ToLower());
            if (isDuplicate)
            {
                return BadRequest(new { message = $"Tên danh mục '{input.Name}' đã tồn tại trên hệ thống!" });
            }

            // Tạo Entity thực tế (CategoryProduct) từ DTO dữ liệu đầu vào
            var newCategory = new CategoryProduct
            {
                Name = input.Name,
                Description = input.Description
            };

            _context.CategoriesProducts.Add(newCategory);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Tạo mới danh mục sản phẩm thành công",
                data = newCategory
            });
        }

        // =====================================
        // UPDATE CATEGORY
        // URL: PUT api/categoriesproduct/5
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryProductInputDto input)
        {
            // 🌟 Đã sửa thành _context.CategoriesProducts
            var category = await _context.CategoriesProducts.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Không tìm thấy danh mục để cập nhật" });
            }

            if (input == null || string.IsNullOrWhiteSpace(input.Name))
            {
                return BadRequest(new { message = "Tên danh mục sản phẩm không được để trống." });
            }

            // 🌟 Đã sửa thành _context.CategoriesProducts để check trùng tên với các bản ghi khác
            var isDuplicate = await _context.CategoriesProducts.AnyAsync(c => c.Name.ToLower() == input.Name.ToLower() && c.Id != id);
            if (isDuplicate)
            {
                return BadRequest(new { message = $"Tên danh mục '{input.Name}' đã bị trùng với một danh mục khác!" });
            }

            // Tiến hành cập nhật
            category.Name = input.Name;
            category.Description = input.Description;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật danh mục sản phẩm thành công",
                data = category
            });
        }

        // =====================================
        // DELETE CATEGORY
        // URL: DELETE api/categoriesproduct/5
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            // 🌟 Đã sửa thành _context.CategoriesProducts
            var category = await _context.CategoriesProducts.FindAsync(id);

            if (category == null)
            {
                return NotFound(new { message = "Không tìm thấy danh mục để xóa" });
            }

            _context.CategoriesProducts.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Đã xóa danh mục sản phẩm thành công (Id: {id})"
            });
        }
    }
}