namespace WarehouseManagement.DTOs.Request
{
    public class CreateWarehouseDto
    {
        public string WareName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string? Email { get; set; }
    }

}
