using System.Text.Json.Serialization;

namespace WarehouseManagement.DTOs.Request;

public class TransferWhDto
{

    public int SourceId { get; set; }
    public int TargetId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public string? Description { get; set; }
    [JsonIgnore]
    public DateTime TransferDate { get; set; } = DateTime.UtcNow;
    [JsonIgnore]
    public string? EmployeeCode { get; set; }
}
