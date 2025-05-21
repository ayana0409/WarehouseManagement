namespace WarehouseManagement.DTOs.Response;

public class ExportDetailDto
{
    public int ExId { get; set; }
    public int ProId { get; set; }
    public string? ProductName { get; set; }
    public int WareId { get; set; }
    public double Quantity { get; set; }
    public double? Price { get; set; }
}

