using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace EmploymentShiftManager.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;

        public HomeController(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            if (User.Identity.IsAuthenticated)
            {
                var user = await _userManager.GetUserAsync(User);
                if (await _userManager.IsInRoleAsync(user, "Admin"))
                    return RedirectToAction("AdminDashboard");
                else if (await _userManager.IsInRoleAsync(user, "NhanVien"))
                    return RedirectToAction("EmployeeDashboard");
            }
            return View();
        }

        public IActionResult AdminDashboard() => View();
        public IActionResult EmployeeDashboard() => View();
    }
}
