using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CMS_Backend.Controllers.api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _config;

        public AuthController(ApplicationDbContext context, IConfiguration config)
        {
            _context = context;
            _config = config;
        }

        // POST /api/auth/login
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
                return BadRequest(new { message = "Vui lòng nhập email và mật khẩu." });

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Email == dto.Email && c.Password == dto.Password);

            if (customer == null)
                return Unauthorized(new { message = "Email hoặc mật khẩu không đúng." });

            var token = GenerateJwtToken(customer);

            return Ok(new
            {
                token,
                user = new
                {
                    customer.Id,
                    customer.FullName,
                    customer.Email,
                    customer.Phone,
                    customer.Address,
                    role = "Customer"
                }
            });
        }

        // GET /api/auth/me
        [HttpGet("me")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> Me()
        {
            var customerId = GetCustomerId();
            if (customerId == null) return Unauthorized(new { message = "Token không hợp lệ." });

            var customer = await _context.Customers
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == customerId);

            if (customer == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            return Ok(new
            {
                customer.Id,
                customer.FullName,
                customer.Email,
                customer.Phone,
                customer.Address,
                role = "Customer"
            });
        }

        // PUT /api/auth/profile — cập nhật thông tin cá nhân
        [HttpPut("profile")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileDto dto)
        {
            var customerId = GetCustomerId();
            if (customerId == null) return Unauthorized(new { message = "Token không hợp lệ." });

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            // Kiểm tra email trùng với người khác
            var emailTaken = await _context.Customers
                .AnyAsync(c => c.Email == dto.Email && c.Id != customerId);
            if (emailTaken)
                return BadRequest(new { message = "Email này đã được sử dụng bởi tài khoản khác." });

            customer.FullName = dto.FullName;
            customer.Email    = dto.Email;
            customer.Phone    = dto.Phone;
            customer.Address  = dto.Address;

            await _context.SaveChangesAsync();

            return Ok(new
            {
                message = "Cập nhật thông tin thành công.",
                user = new
                {
                    customer.Id,
                    customer.FullName,
                    customer.Email,
                    customer.Phone,
                    customer.Address,
                    role = "Customer"
                }
            });
        }

        // POST /api/auth/change-password — đổi mật khẩu
        [HttpPost("change-password")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto dto)
        {
            var customerId = GetCustomerId();
            if (customerId == null) return Unauthorized(new { message = "Token không hợp lệ." });

            var customer = await _context.Customers.FindAsync(customerId);
            if (customer == null)
                return NotFound(new { message = "Không tìm thấy tài khoản." });

            if (customer.Password != dto.OldPassword)
                return BadRequest(new { message = "Mật khẩu hiện tại không đúng." });

            if (dto.NewPassword.Length < 6)
                return BadRequest(new { message = "Mật khẩu mới phải có ít nhất 6 ký tự." });

            customer.Password = dto.NewPassword;
            await _context.SaveChangesAsync();

            return Ok(new { message = "Đổi mật khẩu thành công." });
        }

        // ======================== HELPERS ========================
        private int? GetCustomerId()
        {
            var claim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return int.TryParse(claim, out var id) ? id : null;
        }

        private string GenerateJwtToken(Customer customer)
        {
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_config["Jwt:Key"]!)
            );
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
                new Claim(ClaimTypes.Email, customer.Email),
                new Claim(ClaimTypes.Role, "Customer"),
                new Claim("FullName", customer.FullName ?? "")
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddDays(7),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    public class LoginDto
    {
        public string Email    { get; set; } = "";
        public string Password { get; set; } = "";
    }

    public class UpdateProfileDto
    {
        public string  FullName { get; set; } = "";
        public string  Email    { get; set; } = "";
        public string? Phone    { get; set; }
        public string? Address  { get; set; }
    }

    public class ChangePasswordDto
    {
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string ConfirmPassword { get; set; } = "";
    }
}
