/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
 Mssv: 2123110162
 Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using CMS_Backend.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace CMS.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IEmailService _email;

        public OrdersController(ApplicationDbContext context, IEmailService email)
        {
            _context = context;
            _email   = email;
        }

        #region --- DTOs ---

        public class OrderInputDto
        {
            public int CustomerId { get; set; }
            public string? Notes { get; set; }
        }

        public class OrderUpdateDto
        {
            public int Status { get; set; }
            public string? Notes { get; set; }
        }

        public class CheckoutDto
        {
            public string ShippingAddress { get; set; } = "";
            public string? Notes { get; set; }
            public List<CheckoutItemDto> Items { get; set; } = new();
        }

        public class CheckoutItemDto
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
        }

        #endregion

        // POST /api/orders/checkout — đặt hàng (yêu cầu đăng nhập)
        [HttpPost("checkout")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Checkout([FromBody] CheckoutDto dto)
        {
            if (dto.Items == null || !dto.Items.Any())
                return BadRequest(new { message = "Giỏ hàng không có sản phẩm." });

            if (string.IsNullOrWhiteSpace(dto.ShippingAddress))
                return BadRequest(new { message = "Vui lòng nhập địa chỉ nhận hàng." });

            var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            // Load sản phẩm một lần để validate & trừ tồn kho
            var productIds = dto.Items.Select(i => i.ProductId).Distinct().ToList();
            var products   = await _context.Products
                                           .Where(p => productIds.Contains(p.Id))
                                           .ToListAsync();

            // Kiểm tra tồn kho trước khi tạo đơn
            var stockErrors = new List<string>();
            foreach (var item in dto.Items)
            {
                var prod = products.FirstOrDefault(p => p.Id == item.ProductId);
                if (prod == null)
                {
                    stockErrors.Add($"Sản phẩm ID {item.ProductId} không tồn tại.");
                    continue;
                }
                if (prod.StockQuantity < item.Quantity)
                    stockErrors.Add($"'{prod.Name}' chỉ còn {prod.StockQuantity} sản phẩm trong kho.");
            }
            if (stockErrors.Any())
                return BadRequest(new { message = string.Join(" | ", stockErrors) });

            // Dùng transaction: đảm bảo order + details + stock được lưu cùng lúc
            using var tx = await _context.Database.BeginTransactionAsync();
            try
            {
                // 1. Tạo đơn hàng
                var order = new Order
                {
                    CustomerId      = customerId,
                    ShippingAddress = dto.ShippingAddress,
                    Notes           = dto.Notes,
                    OrderDate       = DateTime.Now,
                    Status          = 0
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync(); // lấy order.Id

                // 2. Thêm chi tiết + trừ tồn kho
                foreach (var item in dto.Items)
                {
                    var prod = products.First(p => p.Id == item.ProductId);

                    _context.OrderDetails.Add(new OrderDetail
                    {
                        OrderId   = order.Id,
                        ProductId = item.ProductId,
                        Quantity  = item.Quantity,
                        UnitPrice = prod.Price
                    });

                    prod.StockQuantity -= item.Quantity; // trừ tồn kho
                }
                await _context.SaveChangesAsync();

                await tx.CommitAsync();

                // Gửi email xác nhận (không chặn response nếu email lỗi)
                var customer = await _context.Customers.FindAsync(customerId);
                if (customer != null)
                {
                    var emailItems = dto.Items.Select(i =>
                    {
                        var p = products.First(x => x.Id == i.ProductId);
                        return new OrderEmailItem
                        {
                            ProductName = p.Name,
                            Quantity    = i.Quantity,
                            UnitPrice   = p.Price
                        };
                    }).ToList();

                    _ = Task.Run(async () =>
                    {
                        try
                        {
                            await _email.SendOrderConfirmationAsync(
                                toEmail:         customer.Email,
                                toName:          customer.FullName,
                                orderId:         order.Id,
                                orderDate:       order.OrderDate.ToString("HH:mm - dd/MM/yyyy"),
                                shippingAddress: dto.ShippingAddress,
                                notes:           dto.Notes,
                                items:           emailItems);
                        }
                        catch { /* email lỗi không ảnh hưởng đơn hàng */ }
                    });
                }

                return Ok(new { message = "Đặt hàng thành công!", orderId = order.Id });
            }
            catch (Exception ex)
            {
                await tx.RollbackAsync();
                return StatusCode(500, new { message = "Lỗi xử lý đơn hàng: " + ex.Message });
            }
        }

        // GET /api/orders/my-orders — lịch sử đơn hàng của khách đang đăng nhập
        [HttpGet("my-orders")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> MyOrders()
        {
            var customerId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

            var orders = await _context.Orders
                .Include(o => o.OrderDetails)
                    .ThenInclude(d => d.Product)
                .Where(o => o.CustomerId == customerId)
                .OrderByDescending(o => o.OrderDate)
                .Select(o => new
                {
                    o.Id,
                    o.OrderDate,
                    o.Status,
                    o.ShippingAddress,
                    o.Notes,
                    TotalAmount = o.OrderDetails!.Sum(d => d.Quantity * d.UnitPrice),
                    ItemCount   = o.OrderDetails!.Count(),
                    Items = o.OrderDetails!.Select(d => new
                    {
                        d.Id,
                        d.ProductId,
                        ProductName  = d.Product != null ? d.Product.Name     : "Không còn kinh doanh",
                        ProductImage = d.Product != null ? d.Product.ImageUrl : "",
                        d.Quantity,
                        d.UnitPrice,
                        SubTotal = d.Quantity * d.UnitPrice
                    })
                })
                .ToListAsync();

            return Ok(orders);
        }

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