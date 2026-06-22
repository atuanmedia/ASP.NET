/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
Mssv: 2123110162
Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CMS.Backend.Controllers
{
    [Authorize]
    public class CategoriesProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CategoriesProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var list = _context.CategoriesProducts.ToList();
            return View(list);
        }

        [HttpGet]
        public IActionResult Create() => View();

        [HttpPost]
        public async Task<IActionResult> Create(CategoryProduct model, IFormFile? uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
                model.ImageUrl = await SaveImage(uploadImage);

            _context.CategoriesProducts.Add(model);
            _context.SaveChanges();
            TempData["Success"] = "Thêm danh mục thành công!";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            var cat = _context.CategoriesProducts.Find(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(CategoryProduct model, IFormFile? uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
            {
                model.ImageUrl = await SaveImage(uploadImage);
            }
            else if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                var old = await _context.CategoriesProducts.AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == model.Id);
                model.ImageUrl = old?.ImageUrl;
            }

            _context.CategoriesProducts.Update(model);
            _context.SaveChanges();
            TempData["Success"] = "Cập nhật danh mục thành công!";
            return RedirectToAction("Index");
        }

        public IActionResult Delete(int id)
        {
            var cat = _context.CategoriesProducts.Find(id);
            if (cat != null)
            {
                _context.CategoriesProducts.Remove(cat);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult Details(int id)
        {
            var cat = _context.CategoriesProducts.Find(id);
            if (cat == null) return NotFound();
            return View(cat);
        }

        private async Task<string> SaveImage(IFormFile file)
        {
            string folder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
            if (!Directory.Exists(folder)) Directory.CreateDirectory(folder);
            string fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            using var stream = new FileStream(Path.Combine(folder, fileName), FileMode.Create);
            await file.CopyToAsync(stream);
            return "/uploads/" + fileName;
        }
    }
}
