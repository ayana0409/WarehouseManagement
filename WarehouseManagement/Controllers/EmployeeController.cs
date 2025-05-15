using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public EmployeeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
            => Ok(await _uow.EmployeeRepository.GetAllAsync(isActive));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _uow.EmployeeRepository.GetDetailAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var updated = await _uow.EmployeeRepository.UpdatePartialAsync(id, dto);
            await _uow.SaveChangesAsync();
            return Ok(updated);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            employee.IsActive = true;
            await _uow.EmployeeRepository.AddAsync(employee);
            await _uow.SaveChangesAsync();
            return Ok(employee.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _uow.EmployeeRepository.FindByIdAsync(id);
            if (employee == null) return NotFound();

            employee.IsActive = false;
            await _uow.SaveChangesAsync();
            return Ok();
        }
    }

}
