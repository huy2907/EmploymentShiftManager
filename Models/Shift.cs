using System;
using System.ComponentModel.DataAnnotations;

namespace EmploymentShiftManager.Models
{
    public class Shift
    {
        public int Id { get; set; }

        [Required]
        public string Title { get; set; }  // Tên ca trực

        [Required]
        public DateTime Start { get; set; } // Ngày giờ bắt đầu

        [Required]
        public DateTime End { get; set; }   // Ngày giờ kết thúc

        public string? AssignedRole { get; set; }
        public string? Color { get; set; }
        public string? Description { get; set; }
    }
}
