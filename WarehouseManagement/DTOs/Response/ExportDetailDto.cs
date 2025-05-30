using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.DTOs.Response;

public class ExportDetailDto
{
    public int Id { get; set; }
    public int ExId { get; set; }
    public int ProId { get; set; }
    public string? ProductName { get; set; }
    public string? WarehouseName { get; set; }
    public int WareId { get; set; }
    public double Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double? Price { get; set; }
}

