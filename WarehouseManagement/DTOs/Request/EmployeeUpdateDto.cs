using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Request
{
    public class EmployeeUpdateDto
    {
        public string? Name { get; set; }
        public string? Code { get; set; }
        public bool? Gender { get; set; }
        public string? Tel { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public RoleEnum? Role { get; set; }
        public bool? IsActive { get; set; }
    }

}
