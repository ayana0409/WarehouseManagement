namespace WarehouseManagement.DTOs.Response;

public class LogDetailDto
{
    public string WhSourceName { get; set; }
    public string WhTargetName { get; set; }
    public string ProductName { get; set; }
    public int ProductId { get; set; }
    public double Quantity { get; set; }
    public string? EmployeeName { get; set; }
    public DateTime CreatedDate { get; set; }
}
