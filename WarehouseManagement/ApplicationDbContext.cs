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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ExportDetail>()
                .HasKey(ed => new { ed.ExId, ed.ProId, ed.WareId });

            modelBuilder.Entity<ImportDetail>()
                .HasKey(id => new { id.ProId, id.ImpId });

            modelBuilder.Entity<WarehouseDetail>()
                .HasKey(wd => new { wd.ProId, wd.WareId });
        }
    }
}
