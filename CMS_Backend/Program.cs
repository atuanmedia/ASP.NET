using CMS.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")
    )
);

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Authentication Cookie
builder.Services.AddAuthentication(
    CookieAuthenticationDefaults.AuthenticationScheme
)
.AddCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

// 🌟 CHỈ GIỮ LẠI 1 CHÍNH SÁCH CORS DUY NHẤT (Đã loại bỏ chính sách "AllowAll" gây đá nhau)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000") // Cho phép ReactJS ở port 3000 gọi tới
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();                  // Hỗ trợ truyền Cookie/Session an toàn
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Swagger middleware
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

// 🌟 VÌ UPLOADS NẰM TRONG WWWROOT, CHỈ CẦN LỆNH NÀY LÀ ĐỦ ĐỂ ĐỌC ẢNH
app.UseStaticFiles();

app.UseRouting();

// 🌟 KÍCH HOẠT CORS ĐÚNG VỊ TRÍ NÀY (Bỏ lệnh UseCors("AllowAll") trùng lặp ở dưới đi)
app.UseCors("AllowReactApp");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}"
);

app.Run();