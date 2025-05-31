namespace WarehouseManagement.DTOs.Response;

public class WarehouseImportExportDailyCountDto
{
    public string Date { get; set; }
    public int ImportCount { get; set; }
    public int ExportCount { get; set; }
    public double ImportPrice { get; set; }
    public double ExportPrice { get; set; }
    public int ImportCompleted { get; set; }
    public int ImportProcessing { get; set; }
    public int ImportNew { get; set; }

    public int ExportCompleted { get; set; }
    public int ExportPending { get; set; }

    public double ImportCompletedRatio { get; set; }  // % completed
    public double ExportCompletedRatio { get; set; }  // % completed
}
