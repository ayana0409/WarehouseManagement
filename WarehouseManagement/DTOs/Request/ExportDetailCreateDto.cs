namespace WarehouseManagement.DTOs.Request;

public class ExportDetailCreateDto
{
    public int ExId { get; set; }
    public int ProId { get; set; }
    public int WareId { get; set; }
    public double Quantity { get; set; }
    public double? Price { get; set; }
}
