namespace WarehouseManagement.DTOs.Request;

public class ImportDetailCreateDto
{
    public int ProId { get; set; }
    public int ImpId { get; set; }
    public double Quantity { get; set; }
    public double Price { get; set; }
    public DateTime ManuDate { get; set; }
}

