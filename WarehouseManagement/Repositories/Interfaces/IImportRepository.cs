using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface IImportRepository : IGenericRepository<Import>
    {
        Task<ImportDto?> GetDetailAsync(int id);
        Task<IEnumerable<ImportDto>> GetAllAsync();
    }
}
