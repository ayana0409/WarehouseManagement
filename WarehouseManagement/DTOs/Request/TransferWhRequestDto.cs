namespace WarehouseManagement.DTOs.Request;

public class TransferWhRequestDto
{
    public IEnumerable<TransferWhDto> Transfers { get; set; } = [];
}
