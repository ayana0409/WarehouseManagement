namespace WarehouseManagement.DTOs.Request
{
    public class ProductUpdateDto
    {
        public string? ProName { get; set; }
        public string? Image { get; set; }
        public string? Unit { get; set; }
        public int? Expiry { get; set; }
        public int? CateId { get; set; }
        public int? ManuId { get; set; }
        public bool? IsActive { get; set; }
    }

}
