using System.ComponentModel.DataAnnotations;

namespace EmploymentShiftManager.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Tên không được bỏ trống")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email không được bỏ trống")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required(ErrorMessage = "Xác nhận mật khẩu không được bỏ trống")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Mật khẩu xác nhận không khớp")]
        public string ConfirmPassword { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Vai trò không được bỏ trống")]
        public string Role { get; set; }
    }
}
