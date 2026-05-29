/*
 Họ Và Tên : Nguyễn Duy Anh Tuấn
Mssv: 2123110162
Lớp : CCQ2311E
*/
using CMS.Data;
using CMS.Data.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
    [Authorize]
    public class CategoriesProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        // Inject DbContext
        public CategoriesProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Hiển thị danh sách CategoriesProducts
        public IActionResult Index()
        {
            // Lấy dữ liệu từ Database
            var categoriesProducts = _context.CategoriesProducts.ToList();

            return View(categoriesProducts);
        }

        // =========================
        // CREATE
        // =========================

        // GET: Hiển thị form thêm mới
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Thêm mới dữ liệu
        [HttpPost]
        public IActionResult Create(CategoryProduct model)
        {
            if (ModelState.IsValid)
            {
                _context.CategoriesProducts.Add(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // =========================
        // EDIT
        // =========================

        // GET: Hiển thị form sửa
        [HttpGet]
        public IActionResult Edit(int id)
        {
            // Tìm category theo id
            var category = _context.CategoriesProducts.Find(id);

            if (category == null)
                return NotFound();

            return View(category);
        }

        // POST: Cập nhật dữ liệu
        [HttpPost]
        public IActionResult Edit(CategoryProduct model)
        {
            if (ModelState.IsValid)
            {
                _context.CategoriesProducts.Update(model);
                _context.SaveChanges();

                return RedirectToAction("Index");
            }

            return View(model);
        }

        // =========================
        // DELETE
        // =========================

        public IActionResult Delete(int id)
        {
            // Tìm category theo id
            var category = _context.CategoriesProducts.Find(id);

            if (category != null)
            {
                // Xóa dữ liệu
                _context.CategoriesProducts.Remove(category);

                // Lưu xuống SQL Server
                _context.SaveChanges();
            }

            return RedirectToAction("Index");
        }

        // =========================
        // DETAILS
        // =========================

        public IActionResult Details(int id)
        {
            var category = _context.CategoriesProducts.Find(id);

            if (category == null)
                return NotFound();

            return View(category);
        }
    }
}