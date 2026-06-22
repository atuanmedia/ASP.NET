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

namespace CMS.Backend.Controllers.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdvertisementsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public AdvertisementsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET /api/advertisements — public, chỉ trả slide đang active, sắp theo SortOrder
        [HttpGet]
        public async Task<IActionResult> GetActive()
        {
            var list = await _context.Advertisements
                .Where(a => a.IsActive)
                .OrderBy(a => a.SortOrder)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Subtitle,
                    a.ButtonText,
                    a.ButtonLink,
                    a.ImageUrl,
                    a.SortOrder
                })
                .ToListAsync();

            return Ok(list);
        }
    }
}
