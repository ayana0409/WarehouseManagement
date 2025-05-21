using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Request
{
    public class ImportUpdateDto
    {
        public int? EmployId { get; set; }
        public ImportEnum? Status { get; set; }
        public string? SupplierName { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }

}
