namespace WarehouseManagement.DTOs.Response
{
    public class ProductDto
    {
        public int Id { get; set; }
        public string ProName { get; set; }
        public string? Image { get; set; }
        public string? Unit { get; set; }
        public int Expiry { get; set; }
        public double Quantity { get; set; }
        public double UnallocatedStock { get; set; }
        public bool IsActive { get; set; }
        public double ImportPrice { get; set; }
        public double ExportPrice { get; set; }
        public DateTime CreatedDate { get; set; }

        public string? ManufacturerName { get; set; }
        public string? CategoryName { get; set; }
    }

}
