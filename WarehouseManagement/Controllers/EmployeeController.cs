using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using WarehouseManagement.DTOs.Request;
using WarehouseManagement.Model;
using WarehouseManagement.Repository.Abtraction;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Controllers
{
    [ApiController]
    [Authorize(Roles = "Admin")]
    [Route("api/[controller]")]
    public class EmployeeController : ControllerBase
    {
        private readonly IUnitOfWork _uow;

        public EmployeeController(IUnitOfWork uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] bool? isActive)
            => Ok(await _uow.EmployeeRepository.GetAllAsync(isActive));

        [HttpGet("{id}")]
        public async Task<IActionResult> GetDetail(int id)
        {
            var result = await _uow.EmployeeRepository.GetDetailAsync(id);
            return result == null ? NotFound() : Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] EmployeeUpdateDto dto)
        {
            var emp = await _uow.EmployeeRepository.FindByIdAsync(id);
            if (emp == null) return BadRequest("Không tìm thấy nhân viên.");

            if (dto.Code != null && dto.Code != emp.Code)
            {
                var existEmployee = await _uow.EmployeeRepository.GetByCode(dto.Code);
                if (existEmployee != null)
                    return BadRequest($"Mã nhân viên {dto.Code} đã tồn tại.");
            }

            if (dto.Name != null) emp.Name = dto.Name;
            if (dto.Code != null) emp.Code = dto.Code;
            if (dto.Gender.HasValue) emp.Gender = dto.Gender.Value;
            if (dto.Tel != null) emp.Tel = dto.Tel;
            if (dto.Email != null) emp.Email = dto.Email;
            if (dto.Address != null) emp.Address = dto.Address;
            if (dto.Role.HasValue) emp.Role = dto.Role.Value;
            if (dto.IsActive.HasValue) emp.IsActive = dto.IsActive;

            _uow.EmployeeRepository.Update(emp);

            var updated = await _uow.EmployeeRepository.UpdatePartialAsync(id, dto);
            await _uow.SaveChangesAsync();
            return Ok(updated);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] Employee employee)
        {
            var existEmployee = await _uow.EmployeeRepository.GetByCode(employee.Code);
            if (existEmployee != null)
                return BadRequest($"Mã nhân viên {employee.Code} đã tồn tại.");

            employee.IsActive = true;
            await _uow.EmployeeRepository.AddAsync(employee);
            await _uow.SaveChangesAsync();
            return Ok(employee.Id);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _uow.EmployeeRepository.FindByIdAsync(id);
            if (employee == null) return NotFound();

            employee.IsActive = false;
            await _uow.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("Roles")]
        public async Task<IActionResult> GetRole()
        {
            var result = new Dictionary<int, string>();
            foreach (var item in Enum.GetValues(typeof(RoleEnum)))
            {
                result.Add((int)item, item.ToString());
            }
            return Ok(result);
        }

        [HttpPut("ChangePassword/{id}")]
        public async Task<IActionResult> ChangePassword(int id, [FromBody] ChangePasswordEmployeeDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
            {
                return BadRequest("Mật khẩu mới và xác nhận mật khẩu không khớp.");
            }

            var employee = await _uow.EmployeeRepository.FindByIdAsync(id);
            if (employee == null) return NotFound();

            if (employee.Password != dto.OldPassword)
            {
                return BadRequest("Mật khẩu cũ không đúng.");
            }

            employee.Password = dto.NewPassword;
            _uow.EmployeeRepository.Update(employee);
            await _uow.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("ResetPassword/{id}")]
        public async Task<IActionResult> ResetPassword(int id)
        {
            var employee = await _uow.EmployeeRepository.FindByIdAsync(id);
            if (employee == null) return NotFound();

            employee.Password = "123456"; // Default password
            _uow.EmployeeRepository.Update(employee);
            await _uow.SaveChangesAsync();
            return Ok();
        }

        // login jwt
        [HttpPost("Login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (string.IsNullOrEmpty(dto.Username) || string.IsNullOrEmpty(dto.Password))
                return BadRequest("Tên đăng nhập và mật khẩu không được để trống.");

            var employee = await _uow.EmployeeRepository.GetByCode(dto.Username);
            if (employee == null)
                return BadRequest("Tài khoản không tồn tại.");

            if (employee.Password != dto.Password)
                return BadRequest("Mật khẩu không đúng.");

            if (employee.IsActive == null || !employee.IsActive.Value)
                return BadRequest("Tài khoản đã bị khóa vĩnh viễn.");

            // Generate JWT token here (not implemented in this snippet)
            var token = GenerateJwtToken(employee);

            return Ok(new
            {
                // Token = token,
                EmployeeId = employee.Id,
                Name = employee.Name,
                Role = employee.Role,
                Token = token
            });
        }

        private string GenerateJwtToken(Employee employee)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes("dh21kpm02-httt-jwtsecretkey@1234567890");

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.NameIdentifier, employee.Id.ToString()),
                    new Claim(ClaimTypes.Name, employee.Name),
                    new Claim(ClaimTypes.Role, employee.Role.ToString())
                }),
                Expires = DateTime.UtcNow.AddDays(7),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
