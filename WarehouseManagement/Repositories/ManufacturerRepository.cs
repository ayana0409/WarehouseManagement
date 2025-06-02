using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class ManufacturerRepository : GenericRepository<Manufacturer>, IManufacturerRepository
    {
        public ManufacturerRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<ManufacturerDto>> GetAllDtoAsync(bool? isActive = null)
        {
            var query = GetAll();

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive.Value);

            return await query.Select(x => new ManufacturerDto
            {
                Id = x.Id,
                ManuName = x.ManuName,
                Address = x.Address,
                Tel = x.Tel,
                Email = x.Email,
                Website = x.Website,
                IsActive = x.IsActive
            }).ToListAsync();
        }

        public async Task<ManufacturerDto?> GetDtoByIdAsync(int id)
        {
            var entity = await FindByIdAsync(id);
            return entity == null ? null : new ManufacturerDto
            {
                Id = entity.Id,
                ManuName = entity.ManuName,
                Address = entity.Address,
                Tel = entity.Tel,
                Email = entity.Email,
                Website = entity.Website,
                IsActive = entity.IsActive
            };
        }
    }

}
