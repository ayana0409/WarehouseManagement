using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class WarehouseDetailRepository : GenericRepository<WarehouseDetail>, IWarehouseDetailRepository
    {
        public WarehouseDetailRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<WarehouseDetail?> GetByIdAsync(int proId, int wareId)
        {
            return await _context.WarehouseDetails.Where(x => x.ProId.Equals(proId) && x.WareId.Equals(wareId)).FirstOrDefaultAsync();
        }

        public async Task<WarehouseDetail?> UpdateAsync(int proId, int wareId, WarehouseDetailUpdateDto dto)
        {
            var entity = await GetByIdAsync(proId, wareId);
            if (entity == null)
                return null;

            if (dto.Quantity.HasValue)
                entity.Quantity = dto.Quantity.Value;

            await _context.SaveChangesAsync();
            return entity;
        }
    }

}
