using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository;

namespace WarehouseManagement.Repositories
{
    public class EmployeeRepository : GenericRepository<Employee>, IEmployeeRepository
    {
        private readonly ApplicationDbContext _context;

        public EmployeeRepository(ApplicationDbContext context) : base(context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EmployeeDto>> GetAllAsync(bool? isActive = null)
        {
            var query = _context.Employees.AsQueryable();

            if (isActive.HasValue)
                query = query.Where(x => x.IsActive == isActive);

            return await query.Select(e => new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Code = e.Code,
                Gender = e.Gender,
                Tel = e.Tel,
                Email = e.Email,
                Address = e.Address,
                RoleName = e.Role.ToString(),
                IsActive = e.IsActive
            }).ToListAsync();
        }

        public async Task<EmployeeDto?> GetDetailAsync(int id)
        {
            var e = await _context.Employees.FirstOrDefaultAsync(x => x.Id == id);
            if (e == null) return null;

            return new EmployeeDto
            {
                Id = e.Id,
                Name = e.Name,
                Code = e.Code,
                Gender = e.Gender,
                Tel = e.Tel,
                Email = e.Email,
                Address = e.Address,
                RoleName = e.Role.ToString(),
                IsActive = e.IsActive
            };
        }

        public async Task<Employee> UpdatePartialAsync(int id, EmployeeUpdateDto dto)
        {
            var emp = await _context.Employees.FindAsync(id);
            if (emp == null) throw new Exception("Employee not found");

            if (dto.Name != null) emp.Name = dto.Name;
            if (dto.Code != null) emp.Code = dto.Code;
            if (dto.Gender.HasValue) emp.Gender = dto.Gender.Value;
            if (dto.Tel != null) emp.Tel = dto.Tel;
            if (dto.Email != null) emp.Email = dto.Email;
            if (dto.Address != null) emp.Address = dto.Address;
            if (dto.Role.HasValue) emp.Role = dto.Role.Value;
            if (dto.IsActive.HasValue) emp.IsActive = dto.IsActive;

            _context.Employees.Update(emp);
            return emp;
        }

        
        public async Task<Employee?> GetByCode(string code)
        {
            return await _context.Employees
                .Where(x => x.Code == code)
                .FirstOrDefaultAsync();
        }
    }

}
