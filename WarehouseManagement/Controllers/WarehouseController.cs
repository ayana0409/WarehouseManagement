﻿using Microsoft.AspNetCore.Mvc;
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
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
        {
            var query = _uow.WarehouseRepository.GetAll();

            if (isActive.HasValue)
                query = query.Where(w => w.IsActive == isActive.Value);

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
        public async Task<IActionResult> GetDetail(int id)
        {
            var warehouse = await _uow.WarehouseRepository.GetWithDetailsAsync(id);
            if (warehouse == null) return NotFound();
            return Ok(warehouse);
        }

        [HttpPost]
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
    }

}
