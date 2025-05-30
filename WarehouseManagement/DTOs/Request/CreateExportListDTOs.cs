using WarehouseManagement.DTOs.Request;

public class CreateExportListDTOs : ExportCreateDto
{
    public IEnumerable<CreateExportDetailListDTOs>? ExportDetails { get; set; }
}