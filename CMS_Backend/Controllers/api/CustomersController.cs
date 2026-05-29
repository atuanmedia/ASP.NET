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
    public class CustomersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region --- DATA TRANSFER OBJECTS (DTOs) ---

        // DTO nhận dữ liệu khi Thêm mới/Đăng ký (Hứng password thô đúng thực thể của bạn)
        public class CustomerInputDto
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Phone { get; set; }    // Cho phép null khớp với Entity
            public string? Address { get; set; }  // Cho phép null khớp với Entity
            public string Password { get; set; }
        }

        // DTO dùng khi Cập nhật thông tin (Không cần truyền lại password)
        public class CustomerUpdateDto
        {
            public string FullName { get; set; }
            public string Email { get; set; }
            public string? Phone { get; set; }
            public string? Address { get; set; }
        }

        #endregion

        // =====================================
        // GET ALL CUSTOMERS
        // URL: GET api/customers
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            // Bảo mật hệ thống: Select gạn lọc bỏ trường Password, không trả về cho client
            var customers = await _context.Customers
                                          .OrderByDescending(c => c.Id)
                                          .Select(c => new
                                          {
                                              c.Id,
                                              c.FullName,
                                              c.Email,
                                              c.Phone,
                                              c.Address
                                          })
                                          .ToListAsync();

            return Ok(customers);
        }

        // =====================================
        // GET CUSTOMER BY ID
        // URL: GET api/customers/5
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var customer = await _context.Customers
                                         .Where(c => c.Id == id)
                                         .Select(c => new
                                         {
                                             c.Id,
                                             c.FullName,
                                             c.Email,
                                             c.Phone,
                                             c.Address
                                         })
                                         .FirstOrDefaultAsync();

            if (customer == null)
            {
                return NotFound(new { message = $"Không tìm thấy khách hàng sở hữu mã ID: {id}" });
            }

            return Ok(customer);
        }

        // =====================================
        // CREATE CUSTOMER
        // URL: POST api/customers
        // =====================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CustomerInputDto input)
        {
            if (input == null)
            {
                return BadRequest(new { message = "Dữ liệu khách hàng gửi lên trống." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Kiểm tra trùng lắp tài khoản Email trong hệ thống
            var isEmailDuplicate = await _context.Customers.AnyAsync(c => c.Email.ToLower() == input.Email.ToLower());
            if (isEmailDuplicate)
            {
                return BadRequest(new { message = $"Địa chỉ Email '{input.Email}' đã được đăng ký sử dụng!" });
            }

            // Gán dữ liệu vào thực thể khách hàng (Lưu password thô theo yêu cầu)
            var newCustomer = new Customer
            {
                FullName = input.FullName,
                Email = input.Email,
                Phone = input.Phone,
                Address = input.Address,
                Password = input.Password
            };

            _context.Customers.Add(newCustomer);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đăng ký thông tin tài khoản khách hàng thành công!",
                data = new { newCustomer.Id, newCustomer.FullName, newCustomer.Email }
            });
        }

        // =====================================
        // UPDATE CUSTOMER
        // URL: PUT api/customers/5
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CustomerUpdateDto input)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy hồ sơ khách hàng để cập nhật." });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Chặn trùng lặp Email nếu khách hàng sửa đổi sang Email mới
            var isEmailDuplicate = await _context.Customers.AnyAsync(c => c.Email.ToLower() == input.Email.ToLower() && c.Id != id);
            if (isEmailDuplicate)
            {
                return BadRequest(new { message = "Email thay đổi đã trùng với một khách hàng khác!" });
            }

            // Cập nhật thông tin sửa đổi
            customer.FullName = input.FullName;
            customer.Email = input.Email;
            customer.Phone = input.Phone;
            customer.Address = input.Address;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật thông tin khách hàng thành công!",
                data = new { customer.Id, customer.FullName }
            });
        }

        // =====================================
        // DELETE CUSTOMER
        // URL: DELETE api/customers/5
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var customer = await _context.Customers.FindAsync(id);

            if (customer == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin khách hàng để tiến hành xóa." });
            }

            _context.Customers.Remove(customer);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã gỡ bỏ thành công tài khoản khách hàng mang Id: {id}" });
        }
    }
}