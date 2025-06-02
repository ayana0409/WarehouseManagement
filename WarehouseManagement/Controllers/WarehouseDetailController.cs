using Microsoft.AspNetCore.Authorization;
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
        [Authorize()]
        public async Task<IActionResult> GetAll()
        {
            var list = await _unitOfWork.WarehouseDetailRepository.GetAll().ToListAsync();
            return Ok(list);
        }

        [HttpGet("{proId}/{wareId}")]
        [Authorize()]
        public async Task<IActionResult> GetById(int proId, int wareId)
        {
            var entity = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(proId, wareId);
            if (entity == null) return NotFound();
            return Ok(entity);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Create([FromBody] WarehouseDetailCreateDto dto)
        {
            var entity = new WarehouseDetail
            {
                ProId = dto.ProId,
                WareId = dto.WareId,
                Quantity = dto.Quantity
            };

            var existDetail = _unitOfWork.WarehouseDetailRepository
            .GetAll(x => x.WareId.Equals(dto.WareId) && x.ProId.Equals(dto.ProId)).FirstOrDefault();

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(dto.ProId);
            if (product.UnallocatedStock < dto.Quantity)
                return BadRequest($"Số lượng chưa phân phối còn lại: {product.UnallocatedStock}");

            if (existDetail == null)
            {
                await _unitOfWork.WarehouseDetailRepository.AddAsync(entity);
                product.UnallocatedStock -= dto.Quantity;
            }
            else
            {
                existDetail.Quantity += dto.Quantity;
                entity = existDetail;
                _unitOfWork.WarehouseDetailRepository.Update(existDetail);
                product.UnallocatedStock += dto.Quantity;
            }
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return Ok(entity);
        }

        [HttpPut("{proId}/{wareId}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Update(int proId, int wareId, [FromBody] WarehouseDetailUpdateDto dto)
        {
            var entity = await _unitOfWork.WarehouseDetailRepository.GetByIdAsync(proId, wareId);
            if (entity == null)
                 return NotFound();

            var product = await _unitOfWork.ProductRepository.GetByIdAsync(proId);
            if (product.UnallocatedStock < dto.Quantity)
                return BadRequest($"Số lượng chưa phân phối còn lại: {product.UnallocatedStock}");

            if (dto.Quantity.HasValue)
                entity.Quantity = dto.Quantity.Value;

            product.UnallocatedStock += dto.Quantity ?? 0;
           
            await _unitOfWork.SaveChangesAsync();
            return Ok(entity);
        }

        [HttpDelete("{proId}/{wareId}")]
        [Authorize(Roles = "Admin, Manager")]
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
