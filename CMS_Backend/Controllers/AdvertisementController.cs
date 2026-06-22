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
    public class AdvertisementController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdvertisementController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Advertisement
        public async Task<IActionResult> Index()
        {
            var list = await _context.Advertisements
                .OrderBy(a => a.SortOrder)
                .ToListAsync();
            return View(list);
        }

        // GET: /Advertisement/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Advertisement { IsActive = true, SortOrder = 1 });
        }

        // POST: /Advertisement/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Advertisement model, IFormFile? uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
                model.ImageUrl = await SaveImage(uploadImage);

            if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                ModelState.AddModelError("ImageUrl", "Vui lòng chọn ảnh hoặc nhập URL ảnh.");
                return View(model);
            }

            model.CreatedAt = DateTime.Now;
            _context.Advertisements.Add(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Thêm banner thành công!";
            return RedirectToAction("Index");
        }

        // GET: /Advertisement/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var ad = await _context.Advertisements.FindAsync(id);
            if (ad == null) return NotFound();
            return View(ad);
        }

        // POST: /Advertisement/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Advertisement model, IFormFile? uploadImage)
        {
            if (uploadImage != null && uploadImage.Length > 0)
            {
                model.ImageUrl = await SaveImage(uploadImage);
            }
            else if (string.IsNullOrWhiteSpace(model.ImageUrl))
            {
                // Giữ lại ảnh cũ nếu không upload và không nhập URL
                var old = await _context.Advertisements.AsNoTracking()
                    .FirstOrDefaultAsync(a => a.Id == model.Id);
                model.ImageUrl = old?.ImageUrl ?? "";
            }

            _context.Advertisements.Update(model);
            await _context.SaveChangesAsync();
            TempData["Success"] = "Cập nhật banner thành công!";
            return RedirectToAction("Index");
        }

        // POST: /Advertisement/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var ad = await _context.Advertisements.FindAsync(id);
            if (ad != null)
            {
                _context.Advertisements.Remove(ad);
                await _context.SaveChangesAsync();
            }
            TempData["Success"] = "Đã xóa banner.";
            return RedirectToAction("Index");
        }

        // POST: /Advertisement/ToggleActive/5
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var ad = await _context.Advertisements.FindAsync(id);
            if (ad != null)
            {
                ad.IsActive = !ad.IsActive;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction("Index");
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
