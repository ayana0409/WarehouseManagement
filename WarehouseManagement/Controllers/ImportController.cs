﻿using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class ImportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ImportController> logger;

        public ImportController(IUnitOfWork unitOfWork, ILogger<ImportController> logger)
        {
            _unitOfWork = unitOfWork;
            this.logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ImportCreateDto dto)
        {
            
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var import = new Import
            {
                Address = dto.Address,
                Tel = dto.Tel,
                SupplierName = dto.SupplierName,
                Status = (ImportEnum)dto.Status,
                Email = dto.Email,
                EmployId = int.Parse(userId)
            };
            await _unitOfWork.ImportRepository.AddAsync(import);
            await _unitOfWork.SaveChangesAsync();
            return Ok(new { import.Id });
        }

        [HttpPost("List")]
        public async Task<IActionResult> CreateList([FromBody] CreateImportListDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var entity = new Import
                {
                    Quantity = dto.Details != null ? dto.Details.Sum(x => x.Quantity) : 0,
                    TotalPrice = dto.Details != null ? dto.Details.Sum(x => x.Price * x.Quantity)! : 0,
                    Tel = dto.Tel,
                    Address = dto.Address,
                    Status = ImportEnum.New,
                    SupplierName = dto.SupplierName,
                    Email = dto.Email,
                    EmployId = int.Parse(userId),
                };

                await _unitOfWork.Repository<Import>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Assuming ExportDetails are part of the request DTO
                if (dto.Details != null && dto.Details.Any())
                {
                    var details = dto.Details.Select(x => new ImportDetail
                    {
                        ProId = x.ProId,
                        Quantity = x.Quantity,
                        Price = x.Price,
                        ImpId = entity.Id,
                        ManuDate = x.ManuDate,
                    });
                    await _unitOfWork.Repository<ImportDetail>().AddRangeAsync(details);
                }

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitAsync();
                return Ok();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(400, "An error occurred while processing your request.");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] ImportUpdateDto dto)
        {
            var import = await _unitOfWork.ImportRepository.FindByIdAsync(id);
            if (import == null) return NotFound();
            
            if (import.Status == ImportEnum.Finished)
                return BadRequest("Không thể cập nhật phiếu nhập đã hoàn thành.");

            // Update only if field is not null
            import.EmployId = dto.EmployId ?? import.EmployId;
            import.Status = dto.Status ?? import.Status;
            import.SupplierName = dto.SupplierName ?? import.SupplierName;
            import.Tel = dto.Tel ?? import.Tel;
            import.Address = dto.Address ?? import.Address;
            import.Email = dto.Email ?? import.Email;
            import.Status = ImportEnum.New;

            _unitOfWork.ImportRepository.Update(import);
            await _unitOfWork.SaveChangesAsync();

            return Ok(import);
        }


        [HttpPut("List/{id}")]
        public async Task<IActionResult> UpdateList(int id, [FromBody] CreateImportListDto dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var import = await _unitOfWork.ImportRepository.FindByIdAsync(id);
                if (import == null) return NotFound();

                if (import.Status == ImportEnum.Finished)
                    return BadRequest("Không thể cập nhật phiếu nhập đã hoàn thành.");
                
                // Update only if field is not null
                import.EmployId = dto.EmployId ?? import.EmployId;
                import.Status = dto.Status ?? import.Status;
                import.SupplierName = dto.SupplierName ?? import.SupplierName;
                import.Tel = dto.Tel ?? import.Tel;
                import.Address = dto.Address ?? import.Address;
                import.Email = dto.Email ?? import.Email;
                import.Status = dto.Status ?? import.Status;

                _unitOfWork.ImportRepository.Update(import);
                await _unitOfWork.SaveChangesAsync();

                if (dto.Details != null)
                {
                    var existDetail = _unitOfWork.ImportDetailRepository.GetAll(x => x.ImpId.Equals(id));
                    _unitOfWork.ImportDetailRepository.DeleteRange(existDetail);
                    await _unitOfWork.SaveChangesAsync();

                    // Assuming ExportDetails are part of the request DTO
                    if (dto.Details != null && dto.Details.Any())
                    {
                        var details = dto.Details.Select(x => new ImportDetail
                        {
                            ProId = x.ProId,
                            Quantity = x.Quantity,
                            Price = x.Price,
                            ImpId = import.Id,
                            ManuDate = x.ManuDate,
                        });
                        await _unitOfWork.Repository<ImportDetail>().AddRangeAsync(details);
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                if (dto.Status == ImportEnum.Finished)
                {
                    var importDetails = _unitOfWork.ImportDetailRepository.GetAll(x => x.ImpId == id);
                    foreach (var detail in importDetails)
                    {
                        var product = await _unitOfWork.ProductRepository.FindByIdAsync(detail.ProId);
                        if (product != null)
                        {
                            product.Quantity += detail.Quantity;
                            product.UnallocatedStock += detail.Quantity;
                            _unitOfWork.ProductRepository.Update(product);
                        }
                        
                    }
                    await _unitOfWork.SaveChangesAsync();
                }

                await _unitOfWork.CommitAsync();
                return Ok();
            }
            catch (Exception)
            {
                await _unitOfWork.RollbackAsync();
                return StatusCode(400, "An error occurred while processing your request.");
            }
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
