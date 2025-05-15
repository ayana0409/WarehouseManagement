using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface IWarehouseRepository : IGenericRepository<Warehouse>
    {
        Task<WarehouseWithDetailsDto?> GetWithDetailsAsync(int id);
    }


}
