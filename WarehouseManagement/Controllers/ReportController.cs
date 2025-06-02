using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Controllers;

[ApiController]
[Authorize]
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
        DateTime startDate = fromDate?.Date ?? DateTime.Now.Date.AddDays(-30);
        DateTime endDate = toDate?.Date ?? DateTime.Now.Date;

        if (startDate > endDate)
            return BadRequest("fromDate cannot be greater than toDate.");

        // ===== Lấy dữ liệu Import =====
        var importList = await _uow.ImportRepository
            .GetAll(id => id.CreateDate >= startDate && id.CreateDate <= endDate.AddDays(1))
            .ToListAsync();

        var importData = importList
            .GroupBy(id => id.CreateDate.Date)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Count = g.Count(),
                    Price = g.Sum(x => x.TotalPrice),
                    New = g.Count(x => x.Status == ImportEnum.New),
                    Processing = g.Count(x => x.Status == ImportEnum.Processing),
                    Completed = g.Count(x => x.Status == ImportEnum.Finished)
                });

        // ===== Lấy dữ liệu Export =====
        var exportList = await _uow.ExportRepository
            .GetAll(e => e.CreateDate >= startDate && e.CreateDate <= endDate.AddDays(1))
            .ToListAsync();

        var exportData = exportList
            .GroupBy(e => e.CreateDate.Date)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Count = g.Count(),
                    Price = g.Sum(x => x.TotalPrice),
                    Pending = g.Count(x => x.Status == ExportEnum.Pending),
                    Completed = g.Count(x => x.Status == ExportEnum.Finished)
                });

        // ===== Tạo danh sách kết quả theo ngày =====
        var dateRange = Enumerable.Range(0, (endDate - startDate).Days + 1)
            .Select(d => startDate.AddDays(d))
            .ToList();

        var result = dateRange.Select(date => new WarehouseImportExportDailyCountDto
        {
            Date = date.ToString("dd/MM/yyyy"),

            ImportCount = importData.ContainsKey(date) ? importData[date].Count : 0,
            ImportPrice = importData.ContainsKey(date) ? importData[date].Price : 0,

            ImportNew = importData.ContainsKey(date) ? importData[date].New : 0,
            ImportProcessing = importData.ContainsKey(date) ? importData[date].Processing : 0,
            ImportCompleted = importData.ContainsKey(date) ? importData[date].Completed : 0,

            ExportCount = exportData.ContainsKey(date) ? exportData[date].Count : 0,
            ExportPrice = exportData.ContainsKey(date) ? exportData[date].Price : 0,

            ExportPending = exportData.ContainsKey(date) ? exportData[date].Pending : 0,
            ExportCompleted = exportData.ContainsKey(date) ? exportData[date].Completed : 0
        }).ToList();

        return Ok(result);
    }

    // 2. Thống kê sản phẩm nhập, xuất, tồn
    [HttpGet("product-inventory")]
    public async Task<IActionResult> GetProductInventoryReport(
    [FromQuery] DateTime? fromDate,
    [FromQuery] DateTime? toDate)
    {
        DateTime startDate = fromDate?.Date ?? DateTime.Now.Date.AddDays(-30);
        DateTime endDate = toDate?.Date ?? DateTime.Now.Date;

        if (startDate > endDate)
            return BadRequest("fromDate cannot be greater than toDate.");

        var products = _uow.ProductRepository.GetAll();

        // Lấy các đơn nhập đã hoàn thành trong khoảng thời gian
        var completedImports = await _uow.ImportRepository.GetAll()
            .Where(i => i.Status == ImportEnum.Finished && i.CreateDate.Date >= startDate && i.CreateDate.Date <= endDate)
            .ToListAsync();

        var importIds = completedImports.Select(i => i.Id).ToList();

        // Lấy các chi tiết nhập liên quan
        var importDetails = await _uow.ImportDetailRepository.GetAll()
            .Where(d => importIds.Contains(d.ImpId))
            .ToListAsync();

        // Lấy các đơn xuất đã hoàn thành trong khoảng thời gian
        var completedExports = await _uow.ExportRepository.GetAll()
            .Where(e => e.Status == ExportEnum.Finished && e.CreateDate.Date >= startDate && e.CreateDate.Date <= endDate)
            .ToListAsync();

        var exportIds = completedExports.Select(e => e.Id).ToList();

        // Lấy các chi tiết xuất liên quan
        var exportDetails = await _uow.ExportDetailRepository.GetAll()
            .Where(d => exportIds.Contains(d.ExId))
            .ToListAsync();

        // Gộp dữ liệu
        var importGroups = importDetails
            .GroupBy(d => d.ProId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Quantity = g.Sum(x => x.Quantity),
                    TotalPrice = g.Sum(x => x.Quantity * x.Price)
                });

        var exportGroups = exportDetails
            .GroupBy(d => d.ProId)
            .ToDictionary(
                g => g.Key,
                g => new
                {
                    Quantity = g.Sum(x => x.Quantity),
                    TotalPrice = g.Sum(x => x.Quantity * x.Price)
                });

        var result = products.Select(p => new ProductInventoryReportDto
        {
            ProId = p.Id,
            ProName = p.ProName,
            RemainingStock = p.Quantity,
            TotalImported = importGroups.ContainsKey(p.Id) ? importGroups[p.Id].Quantity : 0,
            TotalImportPrice = importGroups.ContainsKey(p.Id) ? importGroups[p.Id].TotalPrice : 0,
            TotalExported = exportGroups.ContainsKey(p.Id) ? exportGroups[p.Id].Quantity : 0,
            TotalExportPrice = exportGroups.ContainsKey(p.Id) ? (double)exportGroups[p.Id].TotalPrice : 0
        }).ToList();

        return Ok(result);
    }
}
