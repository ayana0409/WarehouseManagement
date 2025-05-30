using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Response
{
    public class ExportDto
    {
        public int Id { get; set; }
        public string EmployeeName { get; set; }
        public DateTime CreateDate { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ConsumerName { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
        public ExportEnum? Status { get; set; }
        public IEnumerable<ExportDetailDto>? ExportDetails { get; set; }
    }
}
