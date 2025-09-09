using EmploymentShiftManager.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.IO.Font;
using iText.IO.Font.Constants;
using System.IO;
using System.Linq;

namespace EmploymentShiftManager.Controllers
{
    [Authorize(Roles = "Admin")]
    public class ReportController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReportController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 📌 Trang giao diện xuất báo cáo
        public IActionResult Index()
        {
            return View();
        }

        // 📌 Export PDF 
        [HttpGet]
        public IActionResult ExportPDF(DateTime? startDate, DateTime? endDate)
        {
            // Nếu chưa chọn ngày → mặc định lấy tất cả
            var query = _context.Shifts.AsQueryable();
            if (startDate.HasValue && endDate.HasValue)
            {
                query = query.Where(s => s.Start >= startDate && s.End <= endDate);
            }

            var shifts = query.ToList();

            using (var ms = new MemoryStream())
            {
                var writer = new PdfWriter(ms);
                var pdf = new PdfDocument(writer);
                var document = new Document(pdf);

                // 🔹 Load font Times New Roman Unicode
                string fontPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "fonts", "times.ttf");
                if (!System.IO.File.Exists(fontPath))
                {
                    return Content("❌ Font Times New Roman không tồn tại trong wwwroot/fonts. Vui lòng thêm file times.ttf.");
                }
                var timesFont = PdfFontFactory.CreateFont(fontPath, PdfEncodings.IDENTITY_H, PdfFontFactory.EmbeddingStrategy.PREFER_EMBEDDED);

                
                string titleText = "BÁO CÁO CA TRỰC";
                if (startDate.HasValue && endDate.HasValue)
                {
                    titleText += $" ({startDate:dd/MM/yyyy} - {endDate:dd/MM/yyyy})";
                }

                document.Add(new Paragraph(titleText)
                    .SetFont(timesFont)
                    .SetFontSize(18)
                    .SetTextAlignment(TextAlignment.CENTER)
                    .SetMarginBottom(20));

                // 🔹 Tạo bảng
                Table table = new Table(6).UseAllAvailableWidth();
                string[] headers = { "ID", "Tên Ca", "Bắt đầu", "Kết thúc", "Vai trò", "Màu" };

                // Header
                foreach (var header in headers)
                {
                    table.AddHeaderCell(new Cell()
                        .Add(new Paragraph(header).SetFont(timesFont).SetFontColor(ColorConstants.WHITE))
                        .SetBackgroundColor(ColorConstants.BLUE)
                        .SetTextAlignment(TextAlignment.CENTER));
                }

                // Dữ liệu
                foreach (var s in shifts)
                {
                    table.AddCell(new Cell().Add(new Paragraph(s.Id.ToString())).SetFont(timesFont).SetTextAlignment(TextAlignment.CENTER));
                    table.AddCell(new Cell().Add(new Paragraph(s.Title ?? "")).SetFont(timesFont));
                    table.AddCell(new Cell().Add(new Paragraph(s.Start.ToString("dd/MM/yyyy HH:mm"))).SetFont(timesFont));
                    table.AddCell(new Cell().Add(new Paragraph(s.End.ToString("dd/MM/yyyy HH:mm"))).SetFont(timesFont));
                    table.AddCell(new Cell().Add(new Paragraph(s.AssignedRole ?? "")).SetFont(timesFont));
                    table.AddCell(new Cell().Add(new Paragraph(s.Color ?? "")).SetFont(timesFont));
                }

                document.Add(table);
                document.Close();

                return File(ms.ToArray(),
                    "application/pdf",
                    "BaoCaoCaTruc.pdf");
            }
        }
    }
}
