using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.DTOs.Response;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;

namespace WarehouseManagement.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;

        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpPost]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Create([FromBody] ProductCreateDto dto)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var product = new Product
            {
                CateId = dto.CateId,
                ManuId = dto.ManuId,
                ProName = dto.ProName,
                Image = dto.Image,
                Unit = dto.Unit ?? "cái",
                Expiry = dto.Expiry ?? 0,
                IsActive = true,
                ExportPrice = dto.ExportPrice,
                ImportPrice = dto.ImportPrice,
            };

            await _unitOfWork.ProductRepository.AddAsync(product);
            await _unitOfWork.SaveChangesAsync();

            return Ok(product.Id);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Update(int id, [FromBody] ProductUpdateDto dto)
        {
            var product = await _unitOfWork.ProductRepository.FindByIdAsync(id);
            if (product == null) return NotFound();

            // Chỉ cập nhật những trường được truyền
            if (dto.ProName != null) product.ProName = dto.ProName;
            if (dto.Image != null) product.Image = dto.Image;
            if (dto.Unit != null) product.Unit = dto.Unit;
            if (dto.Expiry.HasValue) product.Expiry = dto.Expiry.Value;
            if (dto.CateId.HasValue) product.CateId = dto.CateId.Value;
            if (dto.ManuId.HasValue) product.ManuId = dto.ManuId.Value;
            if (dto.IsActive.HasValue) product.IsActive = dto.IsActive.Value;
            if (dto.ImportPrice != null) product.ImportPrice = (double)dto.ImportPrice;
            if (dto.ExportPrice != null) product.ExportPrice = (double)dto.ExportPrice;


            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return Ok(product);
        }


        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _unitOfWork.ProductRepository.FindByIdAsync(id);
            if (product == null) return NotFound();

            product.IsActive = false;
            _unitOfWork.ProductRepository.Update(product);
            await _unitOfWork.SaveChangesAsync();

            return NoContent();
        }
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive = null, [FromQuery] string? keyWord = null)
        {
            var query = _unitOfWork.ProductRepository
                .GetAll(x => keyWord == null || x.ProName.Contains(keyWord))
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .AsQueryable();

            if (isActive.HasValue)
            {
                query = query.Where(p => p.IsActive == isActive);
            }

            var result = await query
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProName = p.ProName,
                    Image = p.Image,
                    Unit = p.Unit,
                    Expiry = p.Expiry,
                    CreatedDate = p.CreateDate,
                    Quantity = p.Quantity,
                    UnallocatedStock = p.UnallocatedStock,
                    ManufacturerName = p.Manufacturer != null ? p.Manufacturer.ManuName : null,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    IsActive = p.IsActive,
                    ExportPrice = p.ExportPrice,
                    ImportPrice = p.ImportPrice,
                }).ToListAsync();

            return Ok(result);
        }

        [HttpGet("{id}")]
        [Authorize]
        public async Task<IActionResult> GetDetail(int id)
        {
            var product = await _unitOfWork.ProductRepository.GetAll()
                .Include(p => p.Manufacturer)
                .Include(p => p.Category)
                .Where(p => p.Id == id)
                .Select(p => new ProductDto
                {
                    Id = p.Id,
                    ProName = p.ProName,
                    Image = p.Image,
                    Unit = p.Unit,
                    Expiry = p.Expiry,
                    CreatedDate = p.CreateDate,
                    Quantity = p.Quantity,
                    UnallocatedStock = p.UnallocatedStock,
                    ManufacturerName = p.Manufacturer != null ? p.Manufacturer.ManuName : null,
                    CategoryName = p.Category != null ? p.Category.Name : null,
                    ImportPrice= p.ImportPrice,
                    ExportPrice= p.ExportPrice,
                }).FirstOrDefaultAsync();

            if (product == null) return NotFound();
            return Ok(product);
        }
    }
}
