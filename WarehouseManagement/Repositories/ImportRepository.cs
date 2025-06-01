using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class ImportRepository : GenericRepository<Import>, IImportRepository
    {
        private readonly ApplicationDbContext _context;

        public ImportRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<ImportDto?> GetDetailAsync(int id)
        {
            var entity = await _context.Imports.Include(x => x.ImportDetails).ThenInclude(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id.Equals(id));
            return entity == null ? null : new ImportDto
            {
                Id = entity.Id,
                Address = entity.Address,
                CreateDate = entity.CreateDate,
                Email = entity.Email,
                EmployId = entity.EmployId,
                Quantity = entity.Quantity,
                Status = entity.Status,
                SupplierName = entity.SupplierName,
                Tel = entity.Tel,
                TotalPrice = entity.TotalPrice,
                ImportDetails = entity.ImportDetails != null ? entity.ImportDetails.Select(detail => new ImportDetailDto
                {
                    Id = detail.Id,
                    ProId = detail.ProId,
                    ImpId = detail.ImpId,
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    ManuDate = detail.ManuDate,
                    ProductName = detail.Product?.ProName,
                    Unit = detail.Product?.Unit
                }) : null
            };
        }

        public async Task<IEnumerable<ImportDto>> GetAllAsync()
        {
            var list = _context.Imports.AsQueryable();
            return list.Include(x => x.ImportDetails).ThenInclude(x => x.Product).Select(entity => new ImportDto
            {
                Id = entity.Id,
                Address = entity.Address,
                CreateDate = entity.CreateDate,
                Email = entity.Email,
                EmployId = entity.EmployId,
                Quantity = entity.Quantity,
                Status = entity.Status,
                SupplierName = entity.SupplierName,
                Tel = entity.Tel,
                TotalPrice = entity.TotalPrice,
                ImportDetails = entity.ImportDetails != null ? entity.ImportDetails.Select(detail => new ImportDetailDto
                {
                    Id = detail.Id,
                    ProId = detail.ProId,
                    ImpId = detail.ImpId,
                    Quantity = detail.Quantity,
                    Price = detail.Price,
                    ManuDate = detail.ManuDate,
                    ProductName = detail.Product.ProName,
                    Unit = detail.Product.Unit
                }) : null
            }).OrderByDescending(x => x.Id);
        }
    }

}
