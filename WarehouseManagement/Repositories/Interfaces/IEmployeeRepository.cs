using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repositories.Interfaces
{
    public interface IEmployeeRepository : IGenericRepository<Employee>
    {
        Task<IEnumerable<EmployeeDto>> GetAllAsync(bool? isActive = null);
        Task<EmployeeDto?> GetDetailAsync(int id);
        Task<Employee> UpdatePartialAsync(int id, EmployeeUpdateDto dto);
    }

}
