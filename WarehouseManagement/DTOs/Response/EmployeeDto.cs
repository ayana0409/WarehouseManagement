namespace WarehouseManagement.DTOs.Response
{
    public class EmployeeDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool Gender { get; set; }
        public string? Tel { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string RoleName { get; set; }
        public bool? IsActive { get; set; }
    }

}
