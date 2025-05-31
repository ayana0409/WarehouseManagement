using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ExportController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ApplicationDbContext _context;

        public ExportController(IUnitOfWork unitOfWork, ApplicationDbContext context)
        {
            _unitOfWork = unitOfWork;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] ExportCreateDto dto)
        {
            try
            {
                var entity = new Export
                {
                    EmployId = (int)dto.EmployId,
                    Quantity = dto.Quantity,
                    TotalPrice = dto.TotalPrice,
                    ConsumerName = dto.ConsumerName,
                    Tel = dto.Tel,
                    Address = dto.Address,
                    Status = ExportEnum.Pending,
                };

                await _unitOfWork.Repository<Export>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            }
            catch (Exception)
            {
                return StatusCode(400, "An error occurred while processing your request.");
            }

        }

        [HttpPost("List")]
        public async Task<IActionResult> CreateList([FromBody] CreateExportListDTOs dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var entity = new Export
                {
                    EmployId = (int)dto.EmployId,
                    Quantity = dto.ExportDetails != null ? dto.ExportDetails.Sum(x => x.Quantity) : 0,
                    TotalPrice = dto.ExportDetails != null ? dto.ExportDetails.Sum(x => x.Price * x.Quantity)! : 0,
                    ConsumerName = dto.ConsumerName,
                    Tel = dto.Tel,
                    Address = dto.Address,
                    Status = ExportEnum.Pending,
                };

                await _unitOfWork.Repository<Export>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();

                // Assuming ExportDetails are part of the request DTO
                if (dto.ExportDetails != null && dto.ExportDetails.Any())
                {
                    var details = dto.ExportDetails.Select(x => new ExportDetail
                    {
                        ExId = entity.Id,
                        ProId = x.ProId,
                        WareId = x.WareId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    });
                    await _unitOfWork.Repository<ExportDetail>().AddRangeAsync(details);
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
        public async Task<IActionResult> Update(int id, [FromBody] ExportUpdateDto dto)
        {
            var repo = _unitOfWork.Repository<Export>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            if (entity.Status == ExportEnum.Finished)
                return BadRequest("Không thể cập nhật phiếu xuất đã hoàn thành.");

            entity.EmployId = dto.EmployId ?? entity.EmployId;
            entity.Quantity = dto.Quantity ?? entity.Quantity;
            entity.TotalPrice = dto.TotalPrice ?? entity.TotalPrice;
            entity.ConsumerName = dto.ConsumerName ?? entity.ConsumerName;
            entity.Tel = dto.Tel ?? entity.Tel;
            entity.Address = dto.Address ?? entity.Address;
            entity.Status = dto.Status ?? entity.Status;

            repo.Update(entity);
            await _unitOfWork.SaveChangesAsync();

            return Ok();
        }

        [HttpPut("List/{id}")]
        public async Task<IActionResult> UpdateList(int id, [FromBody] CreateExportListDTOs dto)
        {
            try
            {
                await _unitOfWork.BeginTransactionAsync();
                var repo = _unitOfWork.Repository<Export>();
                var entity = await repo.GetByIdAsync(id);
                if (entity == null) return NotFound();

                if (entity.Status == ExportEnum.Finished)
                    return BadRequest("Không thể cập nhật phiếu xuất đã hoàn thành.");

                // Clear existing details if any
                var existingDetails = _unitOfWork.Repository<ExportDetail>().GetAll(d => d.ExId == id);
                if (existingDetails.Any())
                {
                    _unitOfWork.Repository<ExportDetail>().DeleteRange(existingDetails);
                    await _unitOfWork.SaveChangesAsync();
                }

                if (dto.ExportDetails != null && dto.ExportDetails.Any())
                {
                    var details = dto.ExportDetails.Select(x => new ExportDetail
                    {
                        ExId = entity.Id,
                        ProId = x.ProId,
                        WareId = x.WareId,
                        Quantity = x.Quantity,
                        Price = x.Price
                    });
                    await _unitOfWork.Repository<ExportDetail>().AddRangeAsync(details);
                    await _unitOfWork.SaveChangesAsync();
                }

                // Update the main export entity
                entity.EmployId = dto.EmployId ?? entity.EmployId;
                entity.Quantity = dto.ExportDetails != null ? dto.ExportDetails.Sum(x => x.Quantity) : 0;
                entity.TotalPrice = dto.ExportDetails != null ? dto.ExportDetails.Sum(x => x.Price * x.Quantity) : 0;
                entity.ConsumerName = dto.ConsumerName ?? entity.ConsumerName;
                entity.Tel = dto.Tel ?? entity.Tel;
                entity.Address = dto.Address ?? entity.Address;
                entity.Status = dto.Status ?? entity.Status;

                _unitOfWork.Repository<Export>().Update(entity);
                await _unitOfWork.SaveChangesAsync();

                if (entity.Status == ExportEnum.Finished)
                {
                    // Update product stock in warehouses
                    foreach (var detail in dto.ExportDetails)
                    {
                        var warehouseDetail = await _unitOfWork.WarehouseDetailRepository.GetAll()
                            .FirstOrDefaultAsync(x => x.WareId == detail.WareId && x.ProId == detail.ProId);
                        if (warehouseDetail != null)
                        {
                            warehouseDetail.Quantity -= detail.Quantity;
                            _unitOfWork.WarehouseDetailRepository.Update(warehouseDetail);
                        }
                    }
                    await _unitOfWork.SaveChangesAsync();
                    // Update product quantities
                    foreach (var detail in dto.ExportDetails)
                    {
                        var product = await _unitOfWork.ProductRepository.FindByIdAsync(detail.ProId);
                        if (product != null)
                        {
                            product.Quantity -= detail.Quantity;
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

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var repo = _unitOfWork.Repository<Export>();
            var entity = await repo.GetByIdAsync(id);
            if (entity == null) return NotFound();

            repo.Delete(entity);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var entity = await _context.Exports
                .Include(e => e.Employee)
                .Include(e => e.ExportDetails)
                .ThenInclude(d => d.Product)
                .FirstOrDefaultAsync(e => e.Id == id);

            var whs = new List<Warehouse>();
            if (entity.ExportDetails.Any())
                _unitOfWork.WarehouseRepository.GetAll(x => entity.ExportDetails.Select(x => x.WareId).Contains(x.Id));

            if (entity == null) return NotFound();

            var dto = new ExportDto
            {
                Id = entity.Id,
                EmployeeName = entity.Employee?.Name ?? "N/A",
                CreateDate = entity.CreateDate,
                Quantity = entity.Quantity,
                TotalPrice = entity.TotalPrice,
                ConsumerName = entity.ConsumerName,
                Tel = entity.Tel,
                Address = entity.Address,
                Status = entity.Status,
                ExportDetails = entity.ExportDetails?.Select(d => new ExportDetailDto
                {
                    Id = d.Id,
                    ExId = d.ExId,
                    ProId = d.ProId,
                    WareId = d.WareId,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    WarehouseName = whs.Where(x => x.Id.Equals(d.WareId)).FirstOrDefault().WareName ?? null,
                    ProductName = d.Product?.ProName,
                    Unit = d.Product?.Unit ?? string.Empty
                }).ToList() ?? new()
            };

            return Ok(dto);
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var entities = await _unitOfWork.ExportRepository.GetAll()
                .Include(e => e.Employee)
                .Include(e => e.ExportDetails)
                .ThenInclude(d => d.Product)
                .OrderByDescending(x => x.Id)
                .ToListAsync();

            var whs = _unitOfWork.WarehouseRepository.GetAll();

            var detail = _unitOfWork.ExportDetailRepository.GetAll(x => entities.Select(x => x.Id).Contains(x.ExId))
                .Include(x => x.WarehouseInfo)
                .Include(x => x.Product);

            var dtos = entities.Select(entity => new ExportDto
            {
                Id = entity.Id,
                EmployeeName = entity.Employee?.Name ?? "N/A",
                CreateDate = entity.CreateDate,
                Quantity = entity.Quantity,
                TotalPrice = entity.TotalPrice,
                ConsumerName = entity.ConsumerName,
                Tel = entity.Tel,
                Address = entity.Address,
                Status = entity.Status,
                ExportDetails = detail.Where(x => x.ExId.Equals(entity.Id)).Select(d => new ExportDetailDto
                {
                    ExId = d.ExId,
                    ProId = d.ProId,
                    WareId = d.WareId,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    WarehouseName = whs.Where(x => x.Id.Equals(d.WareId)).FirstOrDefault().WareName ?? null,
                    ProductName = d.Product.ProName,
                    Unit = d.Product.Unit ?? string.Empty
                }).ToList() ?? new()
            });

            return Ok(dtos);
        }
    }

}
