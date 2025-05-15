using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repositories;

namespace WarehouseManagement.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();
        private IDbContextTransaction? _transaction;
        public ICategoryRepository CategoryRepository { get; }
        public IManufacturerRepository ManufacturerRepository { get; }
        public IProductRepository ProductRepository { get; private set; }
        public IEmployeeRepository EmployeeRepository { get; }
        public IWarehouseRepository WarehouseRepository { get; }
        public IWarehouseDetailRepository? WarehouseDetailRepository { get; }


        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context;
            CategoryRepository = new CategoryRepository(_context);
            ManufacturerRepository = new ManufacturerRepository(_context);
            ProductRepository = new ProductRepository(_context);
            EmployeeRepository = new EmployeeRepository(_context);
            WarehouseRepository = new WarehouseRepository(_context);
            WarehouseDetailRepository = new WarehouseDetailRepository(_context);
        }

        public IGenericRepository<T> Repository<T>() where T : class
        {
            var type = typeof(T);
            if (!_repositories.ContainsKey(type))
            {
                var repositoryInstance = new GenericRepository<T>(_context);
                _repositories[type] = repositoryInstance;
            }

            return (IGenericRepository<T>)_repositories[type]!;
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public async Task BeginTransactionAsync()
        {
            if (_transaction == null)
            {
                _transaction = await _context.Database.BeginTransactionAsync();
            }
        }

        public async Task CommitAsync()
        {
            if (_transaction != null)
            {
                await _transaction.CommitAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public async Task RollbackAsync()
        {
            if (_transaction != null)
            {
                await _transaction.RollbackAsync();
                await _transaction.DisposeAsync();
                _transaction = null;
            }
        }

        public void Dispose()
        {
            _transaction?.Dispose();
            _context.Dispose();
        }
    }

}
