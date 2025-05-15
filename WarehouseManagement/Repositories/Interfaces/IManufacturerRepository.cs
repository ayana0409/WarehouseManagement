using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface IManufacturerRepository : IGenericRepository<Manufacturer>
    {
        Task<IEnumerable<ManufacturerDto>> GetAllDtoAsync(bool? isActive = null);
        Task<ManufacturerDto?> GetDtoByIdAsync(int id);
    }

}
