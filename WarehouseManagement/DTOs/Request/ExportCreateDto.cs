namespace WarehouseManagement.DTOs.Request
{
    public class ExportCreateDto
    {
        public int EmployId { get; set; }
        public double Quantity { get; set; }
        public double TotalPrice { get; set; }
        public string ConsumerName { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }
    }

}
