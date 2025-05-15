using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface IWarehouseDetailRepository : IGenericRepository<WarehouseDetail>
    {
        Task<WarehouseDetail?> GetByIdAsync(int proId, int wareId);
        Task<WarehouseDetail?> UpdateAsync(int proId, int wareId, WarehouseDetailUpdateDto dto);
    }

}
