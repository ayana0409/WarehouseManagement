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

    [HttpGet("ImportExportSummaryPerDayReport")]
    public async Task<IActionResult> GetImportExportPerDayCount(
        [FromQuery] DateTime? fromDate,
        [FromQuery] DateTime? toDate)
    {
        // Xử lý fromDate và toDate mặc định nếu không được cung cấp
        DateTime startDate = fromDate?.Date ?? DateTime.Now.Date.AddDays(-30); // Mặc định 30 ngày trước
        DateTime endDate = toDate?.Date ?? DateTime.Now.Date; // Mặc định đến hôm nay

        // Đảm bảo startDate không lớn hơn endDate
        if (startDate > endDate)
        {
            return BadRequest("fromDate cannot be greater than toDate.");
        }

        // Lấy dữ liệu Import và Export, nhóm theo ngày
        var importData = _uow.ImportRepository.GetAll(id => 
            id.CreateDate >= startDate && id.CreateDate <= endDate.AddDays(1))
            .GroupBy(id => id.CreateDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                ImportCount = g.Count()
            })
            .ToList();

        var exportData = _uow.ExportRepository.GetAll(id => 
            id.CreateDate >= startDate && id.CreateDate <= endDate.AddDays(1))
            .GroupBy(id => id.CreateDate.Date)
            .Select(g => new
            {
                Date = g.Key,
                ExportCount = g.Count()
            })
            .ToList();

        // Tạo danh sách các ngày trong khoảng thời gian
        var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
            .Select(d => startDate.AddDays(d))
            .ToList();

        // Kết hợp dữ liệu Import và Export theo ngày
        var result = dateRange.Select(date => new WarehouseImportExportDailyCountDto
        {
            Date = date.ToString("dd/MM/yyyy"),
            ImportCount = importData.FirstOrDefault(x => x.Date == date)?.ImportCount ?? 0,
            ExportCount = exportData.FirstOrDefault(x => x.Date == date)?.ExportCount ?? 0
        }).ToList();

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
