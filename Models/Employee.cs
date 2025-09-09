using System.ComponentModel.DataAnnotations;

namespace EmploymentShiftManager.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "T�n nh�n vi�n kh�ng ???c ?? tr?ng")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Ch?c v? kh�ng ???c ?? tr?ng")]
        public string Position { get; set; }

        [Required(ErrorMessage = "Email kh�ng ???c ?? tr?ng")]
        [EmailAddress]
        public string Email { get; set; }

        [Phone]
        public string Phone { get; set; }

        public bool IsActive { get; set; } = true;
    }
}
