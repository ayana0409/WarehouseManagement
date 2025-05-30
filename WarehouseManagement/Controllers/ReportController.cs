using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ReportController : ControllerBase
{
    private readonly IUnitOfWork _uow;

    public ReportController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet("ImportExportSummaryReport")]
    public async Task<IActionResult> GetImportExportCount(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        var importQuantity = _uow.ImportRepository.GetAll(id => (!fromDate.HasValue || id.CreateDate >= fromDate.Value) &&
                                (!toDate.HasValue || id.CreateDate <= toDate.Value)).Count();

        var exportQuantity = _uow.ExportRepository.GetAll(id => (!fromDate.HasValue || id.CreateDate >= fromDate.Value) &&
                                (!toDate.HasValue || id.CreateDate <= toDate.Value)).Count();

        var result = new WarehouseImportExportCountDto
        {
            ImportCount = importQuantity,
            ExportCount = exportQuantity
        };

        return Ok(result);
    }



    // 2. Thống kê sản phẩm nhập, xuất, tồn
    [HttpGet("product-inventory")]
    public async Task<IActionResult> GetProductInventoryReport()
    {
        var products = _uow.ProductRepository.GetAll();

        var importSums = await _uow.ImportDetailRepository.GetAll()
            .GroupBy(x => x.ProId)
            .Select(g => new { ProId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProId, x => x.Quantity);

        var exportSums = await _uow.ExportDetailRepository.GetAll()
            .GroupBy(x => x.ProId)
            .Select(g => new { ProId = g.Key, Quantity = g.Sum(x => x.Quantity) })
            .ToDictionaryAsync(x => x.ProId, x => x.Quantity);

        var result = products.Select(p => new ProductInventoryReportDto
        {
            ProId = p.Id,
            ProName = p.ProName,
            TotalImported = importSums.ContainsKey(p.Id) ? importSums[p.Id] : 0,
            TotalExported = exportSums.ContainsKey(p.Id) ? exportSums[p.Id] : 0,
        }).ToList();

        return Ok(result);
    }
}
