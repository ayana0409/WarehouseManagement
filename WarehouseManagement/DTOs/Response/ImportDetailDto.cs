namespace WarehouseManagement.DTOs.Response;

public class ImportDetailDto
{
    public int Id { get; set; }
    public int ProId { get; set; }
    public string? ProductName { get; set; }
    public int ImpId { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public DateTime ManuDate { get; set; }
}

