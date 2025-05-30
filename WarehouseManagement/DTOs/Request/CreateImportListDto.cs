namespace WarehouseManagement.DTOs.Request
{
    public class CreateImportListDto : ImportCreateDto
    {
        public IEnumerable<ImportDetailCreateDto>? Details { get; set; }
    }
}
