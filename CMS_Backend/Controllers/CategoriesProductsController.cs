using CMS.Data;
using Microsoft.AspNetCore.Mvc;

namespace CMS.Backend.Controllers
{
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
    }
}