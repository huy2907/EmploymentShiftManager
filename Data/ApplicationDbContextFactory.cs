using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EmploymentShiftManager.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Xác định thư mục chứa appsettings.json
            var basePath = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(basePath, "appsettings.json");

            // Nếu appsettings.json không nằm trong thư mục hiện tại
            if (!File.Exists(configPath))
            {
                // Điều chỉnh đường dẫn đến thư mục chứa file
                configPath = Path.Combine(basePath, "EmploymentShiftManager", "appsettings.json");
            }

            // Đọc cấu hình
            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile(configPath, optional: false)
                .Build();

            // Lấy chuỗi kết nối
            var connectionString = configuration.GetConnectionString("DefaultConnection");

            // Cấu hình DbContext
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
