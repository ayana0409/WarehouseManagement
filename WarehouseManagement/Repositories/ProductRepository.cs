using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class ProductRepository : GenericRepository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }
        public async Task<IEnumerable<ProductDto>> GetAllAsync(bool? isActive = null)
        {
            var query = _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .AsQueryable();

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive);

            return await query.Select(p => new ProductDto
            {
                Id = p.Id,
                ProName = p.ProName,
                Image = p.Image,
                Unit = p.Unit,
                Expiry = p.Expiry,
                IsActive = p.IsActive,
                ManufacturerName = p.Manufacturer.ManuName,
                CategoryName = p.Category.Name
            }).ToListAsync();
        }

        public async Task<ProductDto?> GetDetailAsync(int id)
        {
            var p = await _context.Products
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (p == null) return null;

            return new ProductDto
            {
                Id = p.Id,
                ProName = p.ProName,
                Image = p.Image,
                Unit = p.Unit,
                Expiry = p.Expiry,
                IsActive = p.IsActive,
                ManufacturerName = p.Manufacturer.ManuName,
                CategoryName = p.Category.Name
            };
        }

        public async Task<Product> UpdatePartialAsync(int id, ProductUpdateDto dto)
        {
            var product = await _context.Products.FindAsync(id);
            if (product == null) throw new Exception("Product not found");

            if (dto.ProName != null) product.ProName = dto.ProName;
            if (dto.Image != null) product.Image = dto.Image;
            if (dto.Unit != null) product.Unit = dto.Unit;
            if (dto.Expiry.HasValue) product.Expiry = dto.Expiry.Value;
            if (dto.ManuId.HasValue) product.ManuId = dto.ManuId.Value;
            if (dto.CateId.HasValue) product.CateId = dto.CateId.Value;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;

            _context.Products.Update(product);
            return product;
        }
    }

}
