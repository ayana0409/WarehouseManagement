using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Model;

namespace WarehouseManagement
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Export> Exports { get; set; }
        public DbSet<ExportDetail> ExportDetails { get; set; }
        public DbSet<Import> Imports { get; set; }
        public DbSet<ImportDetail> ImportDetails { get; set; }
        public DbSet<Manufacturer> Manufacturers { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<WarehouseDetail> WarehouseDetails { get; set; }
        public DbSet<TransferLog> TransferLogs { get; set; }
        public DbSet<TransferLogDetail> TransferLogDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExportDetail>()
                .HasKey(ed => new { ed.ExId, ed.ProId, ed.WareId });

            modelBuilder.Entity<ImportDetail>()
                .HasKey(id => new { id.ProId, id.ImpId });

            modelBuilder.Entity<WarehouseDetail>()
                .HasKey(wd => new { wd.ProId, wd.WareId });

            modelBuilder.Entity<TransferLogDetail>()
                .HasKey(ld => new { ld.LogId, ld.ProductId });

            // Nếu cần cấu hình TransferLogDetail thì làm tương tự:
            modelBuilder.Entity<TransferLogDetail>()
                .HasOne(d => d.ProductInfo)
                .WithMany() // hoặc .WithMany(p => p.TransferLogDetails)
                .HasForeignKey(d => d.ProductId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false);

            modelBuilder.Entity<WarehouseDetail>()
                .HasOne(wd => wd.Product)
                .WithMany() // hoặc .WithMany(p => p.WarehouseDetails) nếu có
                .HasForeignKey(wd => wd.ProId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false); // Không xoá product nếu bị xoá trong warehouse

            // Cấu hình WarehouseDetail -> Warehouse
            modelBuilder.Entity<WarehouseDetail>()
                .HasOne(wd => wd.Warehouse)
                .WithMany(w => w.WarehouseDetails)
                .HasForeignKey(wd => wd.WareId)
                .OnDelete(DeleteBehavior.Restrict).IsRequired(false); // Hoặc NoAction để tránh cascade loop
        }
    }
}
