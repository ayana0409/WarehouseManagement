namespace WarehouseManagement.DTOs.Response;

public class WarehouseImportExportCountDto
{
    public double ImportCount { get; set; }
    public double ExportCount { get; set; }
}

public class ProductInventoryReportDto
{
    public int ProId { get; set; }
    public string ProName { get; set; }
    public double TotalImported { get; set; }
    public double TotalExported { get; set; }
    public double RemainingStock { get; set; }
    public double TotalImportPrice { get; set; }
    public double TotalExportPrice { get; set; }
}
