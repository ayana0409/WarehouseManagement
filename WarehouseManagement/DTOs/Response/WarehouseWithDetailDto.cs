namespace WarehouseManagement.DTOs.Response
{
    public class WarehouseWithDetailsDto : WarehouseDto
    {
        public IEnumerable<WarehouseDetailDto> Details { get; set; } = new List<WarehouseDetailDto>();
    }

}
