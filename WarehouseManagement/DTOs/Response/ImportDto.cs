using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Response
{
    public class ImportDto
    {
        public int Id { get; set; }
        public int EmployId { get; set; }
        public DateTime CreateDate { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public ImportEnum Status { get; set; }
        public string SupplierName { get; set; }
        public string? Tel { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }
        public IEnumerable<ImportDetailDto>? ImportDetails { get; set; }
    }
}
