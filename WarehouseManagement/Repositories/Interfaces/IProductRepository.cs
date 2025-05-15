using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface IProductRepository : IGenericRepository<Product>
    {
        Task<IEnumerable<ProductDto>> GetAllAsync(bool? isActive = null);
        Task<ProductDto?> GetDetailAsync(int id);
        Task<Product> UpdatePartialAsync(int id, ProductUpdateDto dto);
    }

}
