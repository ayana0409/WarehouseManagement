using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class CategoryRepository : GenericRepository<Category>, ICategoryRepository
    {
        public CategoryRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<CategoryDto>> GetAllDtoAsync(bool? isActive = null)
        {
            var query = GetAll();

            if (isActive.HasValue)
                query = query.Where(c => c.IsActive == isActive.Value);

            return await query.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Image = c.Image
            }).ToListAsync();
        }

        public async Task<CategoryDto?> GetDtoByIdAsync(int id)
        {
            var category = await FindByIdAsync(id);
            return category == null ? null : new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Image = category.Image
            };
        }
    }

}
