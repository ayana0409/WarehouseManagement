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
                    Address = dto.Address
                };

                await _unitOfWork.Repository<Export>().AddAsync(entity);
                await _unitOfWork.SaveChangesAsync();
                return Ok();
            } catch (Exception)
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
                    Address = dto.Address
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

            entity.EmployId = dto.EmployId ?? entity.EmployId;
            entity.Quantity = dto.Quantity ?? entity.Quantity;
            entity.TotalPrice = dto.TotalPrice ?? entity.TotalPrice;
            entity.ConsumerName = dto.ConsumerName ?? entity.ConsumerName;
            entity.Tel = dto.Tel ?? entity.Tel;
            entity.Address = dto.Address ?? entity.Address;

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

                // Clear existing details if any
                var existingDetails = await _unitOfWork.Repository<ExportDetail>().GetAll(d => d.ExId == id).ToListAsync();
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

                _unitOfWork.Repository<Export>().Update(entity);
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
                ExportDetails = entity.ExportDetails?.Select(d => new ExportDetailDto
                {
                    Id = d.Id,
                    ExId = d.ExId,
                    ProId = d.ProId,
                    WareId = d.WareId,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    WarehouseName = d.WarehouseInfo?.WareName,
                    ProductName = d.Product?.ProName
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
                .ToListAsync();

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
                ExportDetails = entity.ExportDetails?.Select(d => new ExportDetailDto
                {
                    ExId = d.ExId,
                    ProId = d.ProId,
                    WareId = d.WareId,
                    Quantity = d.Quantity,
                    Price = d.Price,
                    WarehouseName = d.WarehouseInfo?.WareName,
                    ProductName = d.Product?.ProName
                }).ToList() ?? new()
            });

            return Ok(dtos);
        }
    }

}
