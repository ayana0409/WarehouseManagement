using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ManufacturersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ManufacturersController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null, [FromQuery] string? keyWord = null)
        {
            var result = await _unitOfWork.ManufacturerRepository.GetAllDtoAsync(isActive);

            if (isActive == null && string.IsNullOrEmpty(keyWord))
            {
                result = result.Where(x => keyWord == null ||
                            x.ManuName.Contains(keyWord) ||
                            x.Address.Contains(keyWord) ||
                            x.Tel.Contains(keyWord) ||
                            x.Email.Contains(keyWord) ||
                            x.Website.Contains(keyWord));
                return Ok(result);
            }

            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _unitOfWork.ManufacturerRepository.GetDtoByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ManufacturerCreateUpdateDto dto)
        {
            var entity = new Manufacturer
            {
                ManuName = dto.ManuName,
                Address = dto.Address,
                Tel = dto.Tel,
                Email = dto.Email,
                Website = dto.Website,
                IsActive = true
            };

            await _unitOfWork.ManufacturerRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();

            return Ok(entity.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ManufacturerCreateUpdateDto dto)
        {
            var entity = await _unitOfWork.ManufacturerRepository.FindByIdAsync(id);
            if (entity == null) return NotFound();

            // Chỉ update nếu field được gửi lên
            if (dto.ManuName != null) entity.ManuName = dto.ManuName;
            if (dto.Address != null) entity.Address = dto.Address;
            if (dto.Tel != null) entity.Tel = dto.Tel;
            if (dto.Email != null) entity.Email = dto.Email;
            if (dto.Website != null) entity.Website = dto.Website;

            // Optional: đảm bảo vẫn giữ IsActive = true nếu bạn muốn
            entity.IsActive = true;

            _unitOfWork.ManufacturerRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return Ok(entity);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _unitOfWork.ManufacturerRepository.FindByIdAsync(id);
            if (entity == null) return NotFound();

            entity.IsActive = false;
            _unitOfWork.ManufacturerRepository.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
    }

}
