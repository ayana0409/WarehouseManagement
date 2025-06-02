using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers;

[ApiController]
[Authorize(Roles = "Admin, Manager, Employee")]
[Route("api/[controller]")]
public class ImportDetailController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ApplicationDbContext _context;

    public ImportDetailController(IUnitOfWork unitOfWork, ApplicationDbContext context)
    {
        _unitOfWork = unitOfWork;
        _context = context;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ImportDetailCreateDto dto)
    {
        var entity = new ImportDetail
        {
            ProId = dto.ProId,
            ImpId = dto.ImpId,
            Quantity = dto.Quantity,
            Price = dto.Price,
            ManuDate = dto.ManuDate
        };

        await _unitOfWork.Repository<ImportDetail>().AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();

        return Ok(entity.Id);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ImportDetailUpdateDto dto)
    {
        var repo = _context.Set<ImportDetail>();
        var entity = await repo.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        if (entity == null) return NotFound();

        entity.ProId = dto.ProId ?? entity.ProId;
        entity.ImpId = dto.ImpId ?? entity.ImpId;
        entity.Quantity = dto.Quantity ?? entity.Quantity;
        entity.Price = dto.Price ?? entity.Price;
        entity.ManuDate = dto.ManuDate ?? entity.ManuDate;

        repo.Update(entity);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var repo = _unitOfWork.Repository<ImportDetail>();
        var entity = await repo.GetByIdAsync(id);
        if (entity == null) return NotFound();

        repo.Delete(entity);
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var entity = await _context.ImportDetails
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return NotFound();

        return Ok(new ImportDetailDto
        {
            Id = entity.Id,
            ProId = entity.ProId,
            ProductName = entity.Product?.ProName,
            ImpId = entity.ImpId,
            Quantity = entity.Quantity,
            Price = entity.Price,
            ManuDate = entity.ManuDate
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.ImportDetails.Include(x => x.Product).ToListAsync();

        var result = list.Select(x => new ImportDetailDto
        {
            Id = x.Id,
            ProId = x.ProId,
            ProductName = x.Product?.ProName,
            ImpId = x.ImpId,
            Quantity = x.Quantity,
            Price = x.Price,
            ManuDate = x.ManuDate
        }).ToList();

        return Ok(result);
    }
}

