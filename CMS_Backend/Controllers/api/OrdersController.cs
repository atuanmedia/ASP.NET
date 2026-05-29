/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region --- DATA TRANSFER OBJECTS (DTOs) ---

        // DTO nhận dữ liệu khi Tạo mới đơn hàng
        public class OrderInputDto
        {
            public int CustomerId { get; set; }
            public string? Notes { get; set; }
        }

        // DTO dùng riêng khi Cập nhật trạng thái hoặc thông tin đơn hàng
        public class OrderUpdateDto
        {
            public int Status { get; set; } // 0: Chờ duyệt, 1: Đang giao, 2: Đã xong
            public string? Notes { get; set; }
        }

        #endregion

        // =====================================
        // GET ALL ORDERS
        // URL: GET api/orders
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var orders = await _context.Orders
                                       .Include(o => o.Customer)
                                       .OrderByDescending(o => o.OrderDate)
                                       .Select(o => new
                                       {
                                           o.Id,
                                           o.OrderDate,
                                           o.Status,
                                           o.Notes,
                                           o.CustomerId,
                                           CustomerName = o.Customer != null ? o.Customer.FullName : "Khách vãng lai"
                                       })
                                       .ToListAsync();

            return Ok(orders);
        }

        // =====================================
        // GET ORDER BY ID
        // URL: GET api/orders/5
        // =====================================
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await _context.Orders
                                      .Include(o => o.Customer)
                                      .Where(o => o.Id == id)
                                      .Select(o => new
                                      {
                                          o.Id,
                                          o.OrderDate,
                                          o.Status,
                                          o.Notes,
                                          Customer = o.Customer != null ? new
                                          {
                                              o.Customer.Id,
                                              o.Customer.FullName,
                                              o.Customer.Email,
                                              o.Customer.Phone
                                          } : null
                                      })
                                      .FirstOrDefaultAsync();

            if (order == null)
            {
                return NotFound(new { message = $"Không tìm thấy hóa đơn có mã ID: {id}" });
            }

            return Ok(order);
        }

        // =====================================
        // CREATE ORDER
        // URL: POST api/orders
        // =====================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderInputDto input)
        {
            if (input == null)
            {
                return BadRequest(new { message = "Dữ liệu đơn hàng không hợp lệ." });
            }

            // Kiểm tra xem CustomerId có tồn tại trong hệ thống bảng Customer không
            var isCustomerValid = await _context.Customers.AnyAsync(c => c.Id == input.CustomerId);
            if (!isCustomerValid)
            {
                return BadRequest(new { message = $"Mã khách hàng (CustomerId = {input.CustomerId}) không tồn tại!" });
            }

            // Gán dữ liệu sang thực thể Order thực tế
            var newOrder = new Order
            {
                CustomerId = input.CustomerId,
                Notes = input.Notes,
                OrderDate = DateTime.Now, // Tự động lấy giờ hiện tại
                Status = 0                // Mặc định ban đầu luôn là 0 (Chờ duyệt)
            };

            _context.Orders.Add(newOrder);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Đặt đơn hàng thành công (Chờ ban quản trị duyệt)!",
                data = newOrder
            });
        }

        // =====================================
        // UPDATE ORDER STATUS (Cập nhật trạng thái đơn hàng)
        // URL: PUT api/orders/5
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderUpdateDto input)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy thông tin đơn hàng này." });
            }

            // Kiểm tra tính hợp lệ của Status theo yêu cầu (chỉ nhận 0, 1, 2)
            if (input.Status < 0 || input.Status > 2)
            {
                return BadRequest(new { message = "Trạng thái đơn hàng không hợp lệ! Chỉ nhận (0: Chờ duyệt, 1: Đang giao, 2: Đã xong)." });
            }

            // Cập nhật dữ liệu chỉnh sửa
            order.Status = input.Status;
            order.Notes = input.Notes;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật trạng thái và thông tin đơn hàng thành công!",
                data = order
            });
        }

        // =====================================
        // DELETE ORDER
        // URL: DELETE api/orders/5
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var order = await _context.Orders.FindAsync(id);

            if (order == null)
            {
                return NotFound(new { message = "Không tìm thấy đơn hàng cần hủy bỏ." });
            }

            // Thực hiện xóa đơn hàng
            _context.Orders.Remove(order);
            await _context.SaveChangesAsync();

            return Ok(new { message = $"Đã hủy và xóa hoàn toàn đơn hàng mang mã Id: {id}" });
        }
    }
}