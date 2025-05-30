using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    //[Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public CategoriesController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null)
        {
            var result = await _unitOfWork.CategoryRepository.GetAllDtoAsync(isActive);
            return Ok(result);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _unitOfWork.CategoryRepository.GetDtoByIdAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CategoryCreateDto dto)
        {
            var entity = new Category
            {
                Name = dto.Name,
                Image = dto.Image,
                IsActive = true
            };

            await _unitOfWork.CategoryRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return Ok(entity.Id);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CategoryCreateDto dto)
        {
            var category = await _unitOfWork.CategoryRepository.FindByIdAsync(id);
            if (category == null) return NotFound();

            if (dto.Name != null) category.Name = dto.Name;
            if (dto.Image != null) category.Image = dto.Image;

            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return Ok(category);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _unitOfWork.CategoryRepository.FindByIdAsync(id);
            if (category == null) return NotFound();

            category.IsActive = false;
            _unitOfWork.CategoryRepository.Update(category);
            await _unitOfWork.SaveChangesAsync();
            return NoContent();
        }
    }

}
