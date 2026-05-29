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
    public class ProductsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext vào Controller
        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region --- DATA TRANSFER OBJECTS (DTOs) ---

        // DTO nhận dữ liệu từ Client gửi lên (Bảo vệ dữ liệu, tránh lỗi lặp quan hệ hoặc lỗi 400)
        public class ProductInputDto
        {
            public string Name { get; set; }
            public string Description { get; set; }
            public decimal Price { get; set; }
            public int StockQuantity { get; set; }
            public string ImageUrl { get; set; }
            public int CategoryProductId { get; set; }
        }

        #endregion

        // =====================================
        // GET ALL PRODUCTS
        // URL: GET api/products
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var products = await _context.Products
                                         .Include(p => p.CategoryProduct)
                                         .OrderByDescending(p => p.Id)
                                         .Select(p => new
                                         {
                                             p.Id,
                                             p.Name,
                                             p.Description,
                                             p.Price,
                                             p.StockQuantity,
                                             p.ImageUrl,
                                             p.CategoryProductId,
                                             CategoryProductName = p.CategoryProduct != null ? p.CategoryProduct.Name : "Không xác định"
                                         })
                                         .ToListAsync();

            return Ok(products);
        }

        // =====================================
        // GET PRODUCT BY ID
        // URL: GET api/products/5
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await _context.Products
                                        .Include(p => p.CategoryProduct)
                                        .Where(p => p.Id == id)
                                        .Select(p => new
                                        {
                                            p.Id,
                                            p.Name,
                                            p.Description,
                                            p.Price,
                                            p.StockQuantity,
                                            p.ImageUrl,
                                            p.CategoryProductId,
                                            CategoryProduct = p.CategoryProduct != null ? new
                                            {
                                                p.CategoryProduct.Id,
                                                p.CategoryProduct.Name,
                                                p.CategoryProduct.Description
                                            } : null
                                        })
                                        .FirstOrDefaultAsync();

            if (product == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm có mã Id = {id}" });
            }

            return Ok(product);
        }

        // =====================================
        // GET PRODUCTS BY CATEGORY
        // URL: GET api/products/category/2
        // =====================================
        [HttpGet("category/{categoryId}")]
        public async Task<IActionResult> GetByCategory(int categoryId)
        {
            // Kiểm tra danh mục sản phẩm xem có tồn tại thực tế không
            var categoryExists = await _context.CategoriesProducts.AnyAsync(c => c.Id == categoryId);
            if (!categoryExists)
            {
                return NotFound(new { message = "Danh mục sản phẩm này không tồn tại!" });
            }

            var products = await _context.Products
                                         .Where(p => p.CategoryProductId == categoryId)
                                         .Select(p => new
                                         {
                                             p.Id,
                                             p.Name,
                                             p.Price,
                                             p.StockQuantity,
                                             p.ImageUrl
                                         })
                                         .ToListAsync();

            return Ok(products);
        }

        // =====================================
        // CREATE PRODUCT
        // URL: POST api/products
        // =====================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ProductInputDto input)
        {
            if (input == null)
            {
                return BadRequest(new { message = "Dữ liệu gửi lên không hợp lệ." });
            }

            // Trigger validate ModelState từ Entity cấu hình (ví dụ check Name trống, Price < 0)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra tính hợp lệ của Khóa ngoại CategoryProductId
            var isCategoryValid = await _context.CategoriesProducts.AnyAsync(c => c.Id == input.CategoryProductId);
            if (!isCategoryValid)
            {
                return BadRequest(new { message = $"Mã danh mục sản phẩm (CategoryProductId = {input.CategoryProductId}) không tồn tại!" });
            }

            // Ánh xạ dữ liệu sang Entity thực tế để thêm vào DB
            var newProduct = new Product
            {
                Name = input.Name,
                Description = input.Description,
                Price = input.Price,
                StockQuantity = input.StockQuantity,
                ImageUrl = input.ImageUrl,
                CategoryProductId = input.CategoryProductId
            };

            _context.Products.Add(newProduct);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm sản phẩm mới thành công!",
                data = newProduct
            });
        }

        // =====================================
        // UPDATE PRODUCT
        // URL: PUT api/products/5
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductInputDto input)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = $"Không tìm thấy sản phẩm số {id} để cập nhật." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra khóa ngoại xem có hợp lệ nếu người dùng đổi nhóm danh mục khác
            var isCategoryValid = await _context.CategoriesProducts.AnyAsync(c => c.Id == input.CategoryProductId);
            if (!isCategoryValid)
            {
                return BadRequest(new { message = "Mã danh mục sản phẩm được thay đổi không tồn tại trên hệ thống!" });
            }

            // Cập nhật thông tin mới
            product.Name = input.Name;
            product.Description = input.Description;
            product.Price = input.Price;
            product.StockQuantity = input.StockQuantity;
            product.ImageUrl = input.ImageUrl;
            product.CategoryProductId = input.CategoryProductId;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật thông tin sản phẩm thành công!",
                data = product
            });
        }

        // =====================================
        // DELETE PRODUCT
        // URL: DELETE api/products/5
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _context.Products.FindAsync(id);

            if (product == null)
            {
                return NotFound(new { message = "Không tìm thấy sản phẩm để gỡ bỏ." });
            }

            _context.Products.Remove(product);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = $"Đã xóa thành công sản phẩm có Id: {id}"
            });
        }
    }
}