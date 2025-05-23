using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ExportDetailController : ControllerBase
{
    private readonly ApplicationDbContext _context;
    private readonly IUnitOfWork _unitOfWork;

    public ExportDetailController(
        ApplicationDbContext context,
        IUnitOfWork unitOfWork)
    {
        _context = context;
        _unitOfWork = unitOfWork;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] ExportDetailCreateDto dto)
    {
        var entity = new ExportDetail
        {
            ExId = dto.ExId,
            ProId = dto.ProId,
            WareId = dto.WareId,
            Quantity = dto.Quantity,
            Price = dto.Price
        };

        await _unitOfWork.ExportDetailRepository.AddAsync(entity);
        await _unitOfWork.SaveChangesAsync();
        return Ok();
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(int id, [FromBody] ExportDetailUpdateDto dto)
    {
        var repo = _context.Set<ExportDetail>();
        var entity = await repo.Where(x => x.Id.Equals(id)).FirstOrDefaultAsync();
        if (entity == null) return NotFound();

        entity.Quantity = dto.Quantity ?? entity.Quantity;
        entity.Price = dto.Price ?? entity.Price;

        repo.Update(entity);
        await _context.SaveChangesAsync();

        var productName = await _context.Products
            .Where(p => p.Id == entity.ProId)
            .Select(p => p.ProName)
            .FirstOrDefaultAsync();

        return Ok(new ExportDetailDto
        {
            ExId = entity.ExId,
            ProId = entity.ProId,
            ProductName = productName,
            WareId = entity.WareId,
            Quantity = entity.Quantity,
            Price = entity.Price
        });
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _context.ExportDetails
            .FirstOrDefaultAsync(x => x.Id == id);

        if (entity == null) return NotFound();

        _unitOfWork.ExportDetailRepository.Delete(entity);
        await _unitOfWork.SaveChangesAsync();

        return Ok();
    }

    [HttpGet("{exId}/{proId}")]
    public async Task<IActionResult> GetById(int exId, int proId)
    {
        var entity = await _context.ExportDetails
            .Include(x => x.Product)
            .FirstOrDefaultAsync(x => x.ExId == exId && x.ProId == proId);

        if (entity == null) return NotFound();

        return Ok(new ExportDetailDto
        {
            Id = exId,
            ExId = entity.ExId,
            ProId = entity.ProId,
            ProductName = entity.Product?.ProName,
            WareId = entity.WareId,
            Quantity = entity.Quantity,
            Price = entity.Price
        });
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var list = await _context.ExportDetails
            .Include(x => x.Product)
            .ToListAsync();

        var result = list.Select(entity => new ExportDetailDto
        {
            Id = entity.Id,
            ExId = entity.ExId,
            ProId = entity.ProId,
            ProductName = entity.Product?.ProName,
            WareId = entity.WareId,
            Quantity = entity.Quantity,
            Price = entity.Price
        }).ToList();

        return Ok(result);
    }
}

