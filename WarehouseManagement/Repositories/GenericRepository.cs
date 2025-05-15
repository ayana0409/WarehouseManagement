using Microsoft.EntityFrameworkCore;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }
        public async Task<T?> GetByIdAsync(int id)
        {
            return await _dbSet.FindAsync(id);
        }

        public IQueryable<T> GetAll()
        {
            return _dbSet.AsQueryable();
        }

        public async Task<T?> FindByIdAsync(object id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public async Task AddRangeAsync(IEnumerable<T> entities)
        {
            await _dbSet.AddRangeAsync(entities);
        }

        public void Update(T entity)
        {
            _dbSet.Update(entity);
        }

        public void UpdateRange(IEnumerable<T> entities)
        {
            _dbSet.UpdateRange(entities);
        }

        public void Delete(T entity)
        {
            _dbSet.Remove(entity);
        }
    }

}
