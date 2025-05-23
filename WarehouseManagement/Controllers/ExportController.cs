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
            var entity = new Export
            {
                EmployId = dto.EmployId,
                Quantity = dto.Quantity,
                TotalPrice = dto.TotalPrice,
                ConsumerName = dto.ConsumerName,
                Tel = dto.Tel,
                Address = dto.Address
            };

            await _unitOfWork.Repository<Export>().AddAsync(entity);
            await _unitOfWork.SaveChangesAsync();
            return Ok();
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
                    ExId = d.ExId,
                    ProId = d.ProId,
                    WareId = d.WareId,
                    Quantity = d.Quantity,
                    Price = d.Price,
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
                    ProductName = d.Product?.ProName
                }).ToList() ?? new()
            });

            return Ok(dtos);
        }
    }

}
