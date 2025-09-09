using EmploymentShiftManager.Data;
using EmploymentShiftManager.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmploymentShiftManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly ApplicationDbContext _context;

        public EmployeeController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Danh sách nhân viên
        public async Task<IActionResult> Index()
        {
            ViewBag.SuccessMessage = TempData["SuccessMessage"];
            var employees = await _context.Employees.ToListAsync();
            return View(employees);
        }

        // ================== Tạo nhân viên ==================
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Employee emp)
        {
            if (!ModelState.IsValid)
                return View(emp);

            emp.IsActive = true; // Mặc định nhân viên mới là Active
            _context.Employees.Add(emp);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "✅ Thêm nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================== Sửa nhân viên ==================
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();
            return View(emp);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(Employee emp)
        {
            if (!ModelState.IsValid)
                return View(emp);

            _context.Employees.Update(emp);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "✅ Cập nhật nhân viên thành công!";
            return RedirectToAction(nameof(Index));
        }

        // ================== Chuyển trạng thái Active/Inactive ==================
        [HttpPost]
        public async Task<IActionResult> ToggleActive(int id)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) return NotFound();

            emp.IsActive = !emp.IsActive;
            _context.Update(emp);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = emp.IsActive
                ? "✅ Nhân viên đã được kích hoạt!"
                : "⚠️ Nhân viên đã bị vô hiệu hóa!";

            return RedirectToAction(nameof(Index));
        }
    }
}
