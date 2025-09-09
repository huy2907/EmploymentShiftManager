using EmploymentShiftManager.Data;
using EmploymentShiftManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmploymentShiftManager.Controllers
{
    [Authorize]
    public class CalendarController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CalendarController(ApplicationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ===== Lấy tất cả ca trực =====
        [HttpGet]
        public async Task<IActionResult> GetShifts()
        {
            var shifts = await _context.Shifts.ToListAsync();

            var events = shifts.Select(s => new
            {
                id = s.Id,
                title = s.Title,
                start = s.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = s.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = s.Color,
                role = s.AssignedRole,
                description = s.Description
            });

            return Json(events);
        }

        // ===== Thêm ca trực =====
        [HttpPost]
        public async Task<IActionResult> AddShift([FromBody] Shift shift)
        {
            if (!ModelState.IsValid)
                return BadRequest();

            // Đảm bảo giữ nguyên giờ từ client (FullCalendar trả ISO string)
            shift.Start = DateTime.SpecifyKind(shift.Start, DateTimeKind.Local);
            shift.End = DateTime.SpecifyKind(shift.End, DateTimeKind.Local);

            // Set role & màu tự động
            if (User.IsInRole("Admin"))
            {
                shift.AssignedRole = "Admin";
                shift.Color = "#007bff"; // Xanh dương
            }
            else
            {
                shift.AssignedRole = "NhanVien";
                shift.Color = "#ffc107"; // Vàng
            }

            _context.Shifts.Add(shift);
            await _context.SaveChangesAsync();

            return Json(new
            {
                id = shift.Id,
                title = shift.Title,
                start = shift.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = shift.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = shift.Color,
                role = shift.AssignedRole,
                description = shift.Description
            });
        }

        // ===== Cập nhật ca trực =====
        [HttpPost]
        public async Task<IActionResult> UpdateShift([FromBody] Shift shift)
        {
            var existing = await _context.Shifts.FindAsync(shift.Id);
            if (existing == null)
                return NotFound();

            // Giữ giờ thực từ client
            existing.Title = shift.Title;
            existing.Start = DateTime.SpecifyKind(shift.Start, DateTimeKind.Local);
            existing.End = DateTime.SpecifyKind(shift.End, DateTimeKind.Local);
            existing.Description = shift.Description;

            // Giữ role & màu theo role
            if (User.IsInRole("Admin"))
            {
                existing.AssignedRole = "Admin";
                existing.Color = "#007bff";
            }
            else
            {
                existing.AssignedRole = "NhanVien";
                existing.Color = "#ffc107";
            }

            await _context.SaveChangesAsync();

            return Json(new
            {
                id = existing.Id,
                title = existing.Title,
                start = existing.Start.ToString("yyyy-MM-ddTHH:mm:ss"),
                end = existing.End.ToString("yyyy-MM-ddTHH:mm:ss"),
                color = existing.Color,
                role = existing.AssignedRole,
                description = existing.Description
            });
        }

        // ===== Xóa ca trực =====
        [HttpPost]
        public async Task<IActionResult> DeleteShift(int id)
        {
            try
            {
                var shift = await _context.Shifts.FindAsync(id);
                if (shift == null)
                {
                    return Json(new { success = false, message = "Không tìm thấy ca trực để xóa." });
                }

                _context.Shifts.Remove(shift);
                await _context.SaveChangesAsync();

                return Json(new { success = true, message = "Đã xóa ca trực thành công.", deletedId = id });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Lỗi khi xóa ca trực: " + ex.Message });
            }
        }

    }
}
