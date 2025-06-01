using System.Text.Json.Serialization;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Request
{
    public class ImportCreateDto
    {
        public int? EmployId { get; set; }
        public ImportEnum? Status { get; set; } = ImportEnum.New;
        public string? SupplierName { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
        public string? Email { get; set; }
    }

}
