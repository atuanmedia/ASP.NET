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
    public class OrderDetailsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public OrderDetailsController(ApplicationDbContext context)
        {
            _context = context;
        }

        #region --- DATA TRANSFER OBJECTS (DTOs) ---

        // DTO nhận dữ liệu khi Thêm sản phẩm vào đơn hàng (UnitPrice sẽ tự động lấy từ bảng Product)
        public class OrderDetailInputDto
        {
            public int OrderId { get; set; }
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        // DTO dùng khi muốn thay đổi số lượng sản phẩm trong giỏ hàng/đơn hàng
        public class OrderDetailUpdateDto
        {
            public int Quantity { get; set; }
        }

        #endregion

        // =====================================
        // GET ALL ORDER DETAILS
        // URL: GET api/orderdetails
        // =====================================
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var details = await _context.OrderDetails
                                        .Include(d => d.Product)
                                        .OrderByDescending(d => d.Id)
                                        .Select(d => new
                                        {
                                            d.Id,
                                            d.OrderId,
                                            d.ProductId,
                                            ProductName = d.Product != null ? d.Product.Name : "Sản phẩm đã ngừng kinh doanh",
                                            d.Quantity,
                                            d.UnitPrice,
                                            SubTotal = d.Quantity * d.UnitPrice // Tính tổng tiền của item này luôn
                                        })
                                        .ToListAsync();

            return Ok(details);
        }

        // =====================================
        // GET DETAILS BY ORDER ID (Lấy danh sách sản phẩm thuộc 1 đơn hàng cụ thể)
        // URL: GET api/orderdetails/order/5
        // =====================================
        [HttpGet("order/{orderId}")]
        public async Task<IActionResult> GetByOrderId(int orderId)
        {
            var orderExists = await _context.Orders.AnyAsync(o => o.Id == orderId);
            if (!orderExists)
            {
                return NotFound(new { message = $"Đơn hàng mã số {orderId} không tồn tại trên hệ thống." });
            }

            var details = await _context.OrderDetails
                                        .Include(d => d.Product)
                                        .Where(d => d.OrderId == orderId)
                                        .Select(d => new
                                        {
                                            d.Id,
                                            d.ProductId,
                                            ProductName = d.Product != null ? d.Product.Name : "Sản phẩm đã ngừng kinh doanh",
                                            pImageUrl = d.Product != null ? d.Product.ImageUrl : "",
                                            d.Quantity,
                                            d.UnitPrice,
                                            SubTotal = d.Quantity * d.UnitPrice
                                        })
                                        .ToListAsync();

            return Ok(details);
        }

        // =====================================
        // CREATE ORDER DETAIL (Thêm sản phẩm vào đơn hàng)
        // URL: POST api/orderdetails
        // =====================================
        [HttpPost]
        public async Task<IActionResult> Create([FromBody] OrderDetailInputDto input)
        {
            if (input == null || input.Quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng mặt hàng mua phải lớn hơn 0." });
            }

            // 1. Kiểm tra đơn hàng có tồn tại không
            var isOrderValid = await _context.Orders.AnyAsync(o => o.Id == input.OrderId);
            if (!isOrderValid)
            {
                return BadRequest(new { message = $"Mã đơn hàng (OrderId = {input.OrderId}) không tồn tại!" });
            }

            // 2. Kiểm tra sản phẩm có tồn tại không và lấy giá bán hiện tại của nó
            var product = await _context.Products.FindAsync(input.ProductId);
            if (product == null)
            {
                return BadRequest(new { message = $"Mã sản phẩm (ProductId = {input.ProductId}) không tồn tại!" });
            }

            // 3. Nghiệp vụ nâng cao: Kiểm tra sản phẩm đã có sẵn trong đơn hàng này chưa?
            var existingDetail = await _context.OrderDetails
                                               .FirstOrDefaultAsync(d => d.OrderId == input.OrderId && d.ProductId == input.ProductId);

            if (existingDetail != null)
            {
                // Nếu có rồi, tiến hành cộng dồn số lượng mới vào
                existingDetail.Quantity += input.Quantity;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Sản phẩm đã có sẵn trong đơn, tiến hành cộng dồn số lượng thành công", data = existingDetail });
            }

            // 4. Map sang Entity và lấy giá tự động từ bảng Product
            var newDetail = new OrderDetail
            {
                OrderId = input.OrderId,
                ProductId = input.ProductId,
                Quantity = input.Quantity,
                UnitPrice = product.Price // 🌟 Tự động gán giá sản phẩm tại thời điểm mua
            };

            _context.OrderDetails.Add(newDetail);
            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Thêm sản phẩm vào đơn hàng thành công!",
                data = newDetail
            });
        }

        // =====================================
        // UPDATE QUANTITY (Chỉnh sửa số lượng sản phẩm trong đơn hàng)
        // URL: PUT api/orderdetails/5
        // =====================================
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] OrderDetailUpdateDto input)
        {
            var detail = await _context.OrderDetails.FindAsync(id);

            if (detail == null)
            {
                return NotFound(new { message = "Không tìm thấy chi tiết đơn hàng cần chỉnh sửa." });
            }

            if (input == null || input.Quantity <= 0)
            {
                return BadRequest(new { message = "Số lượng chỉnh sửa phải lớn hơn 0." });
            }

            // Cập nhật số lượng mới
            detail.Quantity = input.Quantity;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật số lượng mặt hàng thành công!",
                data = detail
            });
        }

        // =====================================
        // DELETE ORDER DETAIL (Xóa sản phẩm khỏi đơn hàng)
        // URL: DELETE api/orderdetails/5
        // =====================================
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var detail = await _context.OrderDetails.FindAsync(id);

            if (detail == null)
            {
                return NotFound(new { message = "Không tìm thấy mặt hàng này trong đơn hàng." });
            }

            _context.OrderDetails.Remove(detail);
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đã gỡ bỏ mặt hàng ra khỏi đơn hàng thành công." });
        }
    }
}