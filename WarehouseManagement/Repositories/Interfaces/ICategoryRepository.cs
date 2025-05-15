using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface ICategoryRepository : IGenericRepository<Category>
    {
        Task<IEnumerable<CategoryDto>> GetAllDtoAsync(bool? isActive = null);
        Task<CategoryDto?> GetDtoByIdAsync(int id);
    }

}
