using WarehouseManagement.Repositories.Interfaces;

namespace WarehouseManagement.Repository.Abtraction
{
    public interface IUnitOfWork : IDisposable
    {
        IGenericRepository<T> Repository<T>() where T : class;
        Task<int> SaveChangesAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
        ICategoryRepository CategoryRepository { get; }
        IManufacturerRepository ManufacturerRepository { get; }
        IProductRepository ProductRepository { get; }
        IEmployeeRepository EmployeeRepository { get; }
        IWarehouseRepository WarehouseRepository { get; }
        IWarehouseDetailRepository? WarehouseDetailRepository { get; }
    }
}
