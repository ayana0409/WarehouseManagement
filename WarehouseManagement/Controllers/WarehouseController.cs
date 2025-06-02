using System.Security.Claims;
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
    [Route("api/[controller]")]
    public class WarehouseController : ControllerBase
    {
        private readonly IUnitOfWork _uow;
        public WarehouseController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            var warehouse = await _uow.WarehouseRepository.GetWithDetailsAsync(id);
            if (warehouse == null) return NotFound();
            return Ok(warehouse);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
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
        [Authorize(Roles = "Admin, Manager")]
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
        [Authorize(Roles = "Admin, Manager")]
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
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Transfer([FromBody] TransferWhRequestDto transferRequest)
        {
            if (transferRequest.TransferDetails == null || !transferRequest.TransferDetails.Any())
                return BadRequest("Danh sách chuyển kho không được để trống.");
            await _uow.BeginTransactionAsync();
            try
            {

                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                var transferLog = new TransferLog
                {
                    WhSourceId = transferRequest.SourceId,
                    WhTargetId = transferRequest.TargetId,
                    CreatedDate = DateTime.Now,
                    Description = transferRequest.Description,
                    EmployeeId = int.Parse(userId),
                };
                await _uow.TransferRepository.AddAsync(transferLog);
                await _uow.SaveChangesAsync();

                var addLogDetails = new List<TransferLogDetail>();
                foreach (var transfer in transferRequest.TransferDetails)
                {
                    var sourceDetail = _uow.WarehouseDetailRepository.GetAll(x => x.WareId == transferRequest.SourceId && x.ProId == transfer.ProductId).FirstOrDefault();
                    var targetDetail = _uow.WarehouseDetailRepository.GetAll(x => x.WareId == transferRequest.TargetId && x.ProId == transfer.ProductId).FirstOrDefault();

                    if (sourceDetail == null)
                        return BadRequest($"Không tìm thấy sản phẩm {transfer.ProductId} trong kho {transferRequest.SourceId}.");

                    // if (sourceDetail.Quantity < transfer.Quantity)
                    //     return BadRequest($"Không đủ số lượng sản phẩm {transfer.ProductId} trong kho {transfer.SourceId} để chuyển.");

                    if (targetDetail == null)
                    {
                        targetDetail = new WarehouseDetail
                        {
                            WareId = transferRequest.TargetId,
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

                    addLogDetails.Add(new TransferLogDetail
                    {
                        LogId = transferLog.Id,
                        ProductId = transfer.ProductId,
                        Quantity = transfer.Quantity
                    });

                }
                await _uow.Repository<TransferLogDetail>().AddRangeAsync(addLogDetails);
                await _uow.SaveChangesAsync();
                await _uow.CommitAsync();
                return Ok("Chuyển kho thành công.");
            }
            catch (Exception ex)
            {
                await _uow.RollbackAsync();
                return BadRequest($"Lỗi khi chuyển kho: {ex.Message}");
            }


        }
        [HttpGet("logs")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetTransferLogs([FromQuery] int? sourceId, [FromQuery] int? targetId, [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            // 1. Lấy danh sách TransferLogs để lọc trước
            var logsQuery = _uow.TransferRepository.GetAll();

            if (sourceId.HasValue)
                logsQuery = logsQuery.Where(x => x.WhSourceId == sourceId.Value);
            if (targetId.HasValue)
                logsQuery = logsQuery.Where(x => x.WhTargetId == targetId.Value);
            if (fromDate.HasValue)
                logsQuery = logsQuery.Where(x => x.CreatedDate >= fromDate.Value);
            if (toDate.HasValue)
                logsQuery = logsQuery.Where(x => x.CreatedDate <= toDate.Value);

            var logs = await logsQuery.ToListAsync();
            var logIds = logs.Select(x => x.Id).ToList();

            // 2. Lấy TransferLogDetails theo các LogId đã lọc
            var details = await _uow.Repository<TransferLogDetail>()
                .GetAll(x => logIds.Contains(x.LogId))
                .ToListAsync();

            // 3. Lấy tất cả Id liên quan để truy vấn các bảng khác
            var productIds = details.Select(d => d.ProductId).Distinct().ToList();
            var warehouseIds = logs.SelectMany(x => new[] { x.WhSourceId, x.WhTargetId }).Distinct().ToList();
            var employeeIds = logs.Where(x => x.EmployeeId.HasValue).Select(x => x.EmployeeId!.Value).Distinct().ToList();

            // 4. Truy vấn các bảng khác
            var products = _uow.Repository<Product>().GetAll(x => productIds.Contains(x.Id)).ToDictionary(x => x.Id);
            var warehouses = _uow.WarehouseRepository.GetAll(x => warehouseIds.Contains(x.Id)).ToDictionary(x => x.Id);
            var employees = _uow.EmployeeRepository.GetAll(x => employeeIds.Contains(x.Id)).ToDictionary(x => x.Id);

            // 5. Nối dữ liệu thủ công
            var result = details.Select(detail =>
            {
                var log = logs.FirstOrDefault(l => l.Id == detail.LogId);

                return new LogDetailDto
                {
                    ProductId = detail.ProductId,
                    ProductName = products.TryGetValue(detail.ProductId, out var product) ? product.ProName : "Không xác định",
                    Quantity = detail.Quantity,
                    WhSourceName = log != null && warehouses.TryGetValue(log.WhSourceId, out var src) ? src.WareName : "Không xác định",
                    WhTargetName = log != null && warehouses.TryGetValue(log.WhTargetId, out var tgt) ? tgt.WareName : "Không xác định",
                    EmployeeName = log != null && log.EmployeeId.HasValue && employees.TryGetValue(log.EmployeeId.Value, out var emp)
                        ? emp.Name
                        : "Không xác định",
                    CreatedDate = log?.CreatedDate ?? DateTime.Now
                };
            }).ToList();

            return Ok(result);
        }

    }

}
