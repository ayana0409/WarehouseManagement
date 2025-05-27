using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repositories.Interfaces;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ImportController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImportCreateDto dto)
        {
            var import = new Import
            {
                Address = dto.Address,
                Tel = dto.Tel,
                SupplierName = dto.SupplierName,
                Status = dto.Status,
                Email = dto.Email,
                EmployId = dto.EmployId,

            };
            await _unitOfWork.ImportRepository.AddAsync(import);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { import.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ImportUpdateDto dto)
        {
            var import = await _unitOfWork.ImportRepository.FindByIdAsync(id);
            if (import == null) return NotFound();

            // Update only if field is not null
            import.EmployId = dto.EmployId ?? import.EmployId;
            import.Status = dto.Status ?? import.Status;
            import.SupplierName = dto.SupplierName ?? import.SupplierName;
            import.Tel = dto.Tel ?? import.Tel;
            import.Address = dto.Address ?? import.Address;
            import.Email = dto.Email ?? import.Email;

            _unitOfWork.ImportRepository.Update(import);
            await _unitOfWork.SaveChangesAsync();

            return Ok(import);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _unitOfWork.ImportRepository.GetDetailAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _unitOfWork.ImportRepository.GetAllAsync();
            return Ok(list);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var import = await _unitOfWork.ImportRepository.FindByIdAsync(id);
            if (import == null) return NotFound();

            _unitOfWork.ImportRepository.Delete(import);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }
    }

}
