namespace WarehouseManagement.DTOs.Response
{
    public class WarehouseDto
    {
        public int Id { get; set; }
        public string WareName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
    }

}
