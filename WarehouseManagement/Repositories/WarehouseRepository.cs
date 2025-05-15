using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class WarehouseRepository : GenericRepository<Warehouse>, IWarehouseRepository
    {
        private readonly ApplicationDbContext _context;
        public WarehouseRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<WarehouseWithDetailsDto?> GetWithDetailsAsync(int id)
        {
            return await _context.Warehouses
                .Where(w => w.Id == id)
                .Include(w => w.WarehouseDetails!)
                .ThenInclude(d => d.Product)
                .Select(w => new WarehouseWithDetailsDto
                {
                    Id = w.Id,
                    WareName = w.WareName,
                    Address = w.Address,
                    Tel = w.Tel,
                    Email = w.Email,
                    IsActive = w.IsActive,
                    Details = w.WarehouseDetails!.Select(d => new WarehouseDetailDto
                    {
                        ProductId = d.ProId,
                        ProductName = d.Product!.ProName,
                        Quantity = d.Quantity
                    }).ToList()
                })
                .FirstOrDefaultAsync();
        }
    }

}
