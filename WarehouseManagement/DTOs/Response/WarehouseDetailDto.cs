using System.Text.Json.Serialization;

namespace WarehouseManagement.DTOs.Response
{
    public class WarehouseDetailDto
    {
        public int ProductId { get; set; }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
        public int? WareId { get; set; }
        public string ProductName { get; set; }
        public string Image { get; set; }
        public double Quantity { get; set; }
    }
}
