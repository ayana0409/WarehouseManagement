using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WarehouseDetailController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public WarehouseDetailController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var list = await _unitOfWork.WarehouseDetailRepository.GetAll().ToListAsync();
            return Ok(list);
        }

        [HttpGet("{proId}/{wareId}")]
        public async Task<IActionResult> GetById(int proId, int wareId)
        {
            var entity = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(proId, wareId);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] WarehouseDetailCreateDto dto)
        {
            var entity = new WarehouseDetail
            {
                ProId = dto.ProId,
                WareId = dto.WareId,
                Quantity = dto.Quantity
            };

            await _unitOfWork.WarehouseDetailRepository.AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok(entity);
        }

        [HttpPut("{proId}/{wareId}")]
        public async Task<IActionResult> Update(int proId, int wareId, [FromBody] WarehouseDetailUpdateDto dto)
        {
            var updatedEntity = await _unitOfWork.WarehouseDetailRepository.UpdateAsync(proId, wareId, dto);
            if (updatedEntity == null) return NotFound();
            return Ok(updatedEntity);
        }

        [HttpDelete("{proId}/{wareId}")]
        public async Task<IActionResult> Delete(int proId, int wareId)
        {
            var entity = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(proId, wareId);
            if (entity == null) return NotFound();

            _unitOfWork.WarehouseDetailRepository.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok();
        }
    }

}
