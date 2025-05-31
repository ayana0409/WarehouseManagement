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
    //[Authorize]
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public WarehouseController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive, [FromQuery] string? keyWord)
        {
            var query = _uow.WarehouseRepository.GetAll();

            if (isActive.HasValue)
                query = query.Where(w => w.IsActive == isActive.Value);

            if (!string.IsNullOrEmpty(keyWord))
                query = query.Where(w => w.WareName.Contains(keyWord) || w.Address.Contains(keyWord) || w.Tel.Contains(keyWord) || (w.Email != null && w.Email.Contains(keyWord)));

            var list = await query.Select(w => new WarehouseDto
            {
                Id = w.Id,
                WareName = w.WareName,
                Address = w.Address,
                Tel = w.Tel,
                Email = w.Email,
                IsActive = w.IsActive
            }).ToListAsync();

            return Ok(list);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var warehouse = await _uow.WarehouseRepository.GetWithDetailsAsync(id);
            if (warehouse == null) return NotFound();
            return Ok(warehouse);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateWarehouseDto dto)
        {
            var warehouse = new Warehouse
            {
                WareName = dto.WareName,
                Address = dto.Address,
                Tel = dto.Tel,
                Email = dto.Email,
                IsActive = true
            };

            await _uow.WarehouseRepository.AddAsync(warehouse);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return Ok(new { warehouse.Id });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateWarehouseDto dto)
        {
            var warehouse = await _uow.WarehouseRepository.GetByIdAsync(id);
            if (warehouse == null) return NotFound();

            if (!string.IsNullOrEmpty(dto.WareName)) warehouse.WareName = dto.WareName;
            if (!string.IsNullOrEmpty(dto.Address)) warehouse.Address = dto.Address;
            if (!string.IsNullOrEmpty(dto.Tel)) warehouse.Tel = dto.Tel;
            if (dto.Email != null) warehouse.Email = dto.Email;
            if (dto.IsActive.HasValue) warehouse.IsActive = dto.IsActive.Value;

            _uow.WarehouseRepository.Update(warehouse);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return Ok(warehouse);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var warehouse = await _uow.WarehouseRepository.GetByIdAsync(id);
            if (warehouse == null) return NotFound();

            warehouse.IsActive = false;

            _uow.WarehouseRepository.Update(warehouse);
            await _uow.SaveChangesAsync();
            await _uow.CommitAsync();

            return NoContent();
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> Transfer([FromBody] TransferWhRequestDto transferRequest)
        {
            if (transferRequest.Transfers == null || !transferRequest.Transfers.Any())
                return BadRequest("Danh sách chuyển kho không được để trống.");
            await _uow.BeginTransactionAsync();
            try
            {
                foreach (var transfer in transferRequest.Transfers)
                {
                    var sourceDetail = _uow.WarehouseDetailRepository.GetAll(x => x.WareId == transfer.SourceId && x.ProId == transfer.ProductId).FirstOrDefault();
                    var targetDetail = _uow.WarehouseDetailRepository.GetAll(x => x.WareId == transfer.TargetId && x.ProId == transfer.ProductId).FirstOrDefault();

                    if (sourceDetail == null)
                        return BadRequest($"Không tìm thấy sản phẩm {transfer.ProductId} trong kho {transfer.SourceId}.");
                        
                    // if (sourceDetail.Quantity < transfer.Quantity)
                    //     return BadRequest($"Không đủ số lượng sản phẩm {transfer.ProductId} trong kho {transfer.SourceId} để chuyển.");

                    if (targetDetail == null)
                    {
                        targetDetail = new WarehouseDetail
                        {
                            WareId = transfer.TargetId,
                            ProId = transfer.ProductId,
                            Quantity = transfer.Quantity,
                        };
                        await _uow.WarehouseDetailRepository.AddAsync(targetDetail);
                    }
                    else
                    {
                        targetDetail.Quantity += transfer.Quantity;
                        _uow.WarehouseDetailRepository.Update(targetDetail);
                    }
                    sourceDetail.Quantity -= transfer.Quantity;
                    _uow.WarehouseDetailRepository.Update(sourceDetail);

                    await _uow.SaveChangesAsync();
                }
                await _uow.CommitAsync();
                return Ok("Chuyển kho thành công.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return BadRequest($"Lỗi khi chuyển kho: {ex.Message}");
            }

            
        }
    }

}
