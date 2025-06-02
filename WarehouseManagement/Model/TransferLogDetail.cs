using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace WarehouseManagement.Model
{
    public class TransferLogDetail
    {
        public int LogId { get; set; }
        public int ProductId { get; set; }
        public double Quantity { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProductId))]
        public Product? ProductInfo { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(LogId))]
        public TransferLog? LogInfo { get; set; }
    }
}
