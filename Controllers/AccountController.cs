using EmploymentShiftManager.Data;
using EmploymentShiftManager.Models;
using EmploymentShiftManager.Models.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmploymentShiftManager.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly ApplicationDbContext _context;

        public AccountController(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // =============== ĐĂNG KÝ ====================
        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var user = new IdentityUser { UserName = model.Email, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                string role = model.Role == "Admin" ? "Admin" : "NhanVien";
                await _userManager.AddToRoleAsync(user, role);

                // Tạo Employee mặc định Active
                var emp = new Employee
                {
                    Name = model.Name,
                    Email = model.Email,
                    Position = "Nhân viên",
                    Phone = model.Phone,
                    IsActive = true // ✅ Mặc định kích hoạt
                };
                _context.Employees.Add(emp);
                await _context.SaveChangesAsync();

                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError("", error.Description);

            return View(model);
        }


        // =============== ĐĂNG NHẬP ====================
        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            // Lấy thông tin user
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                ModelState.AddModelError("", "Tài khoản không tồn tại.");
                return View(model);
            }

            // Kiểm tra trạng thái Active trong bảng Employees
            var employee = await _context.Employees.FirstOrDefaultAsync(e => e.Email == model.Email);
            if (employee != null && !employee.IsActive)
            {
                ModelState.AddModelError("", "Tài khoản của bạn đã bị vô hiệu hóa. Vui lòng liên hệ quản trị viên.");
                return View(model);
            }

            // Đăng nhập
            var result = await _signInManager.PasswordSignInAsync(
                model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
                return RedirectToAction("Index", "Home");

            ModelState.AddModelError("", "Đăng nhập thất bại. Vui lòng kiểm tra lại.");
            return View(model);
        }


        // =============== ĐĂNG XUẤT ====================
        [HttpPost]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // =============== THÔNG TIN CÁ NHÂN ====================
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var user = await _userManager.GetUserAsync(User);

            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.Email == user.Email);

            if (employee == null)
            {
                employee = new Employee
                {
                    Name = user.UserName ?? "Chưa cập nhật",
                    Email = user.Email,
                    Position = User.IsInRole("Admin") ? "Admin" : "Nhân viên",
                    Phone = ""
                };
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();
            }

            return View(employee);
        }

        // =============== CHỈNH SỬA THÔNG TIN ====================
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> EditProfile()
        {
            var user = await _userManager.GetUserAsync(User);
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Email == user.Email);
            return View(employee);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditProfile(Employee updated)
        {
            if (!ModelState.IsValid)
                return View(updated);

            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.Id == updated.Id);

            if (employee != null)
            {
                employee.Name = updated.Name;
                employee.Position = updated.Position;
                employee.Phone = updated.Phone;

                _context.Employees.Update(employee);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật thông tin thành công!";
            }

            return RedirectToAction("Profile");
        }
    }
}
