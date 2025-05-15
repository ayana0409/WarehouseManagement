namespace WarehouseManagement.Repository.Abtraction
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> GetAll();
        Task<T?> FindByIdAsync(object id); 
        Task<T?> GetByIdAsync(int id);
        Task AddAsync(T entity);
        Task AddRangeAsync(IEnumerable<T> entities);
        void Update(T entity);
        void UpdateRange(IEnumerable<T> entities);
        void Delete(T entity);
    }

}
