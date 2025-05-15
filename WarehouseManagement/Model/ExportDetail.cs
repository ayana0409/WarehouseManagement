using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WarehouseManagement.Model
{
    public class ExportDetail
    {
        public int ExId { get; set; }

        public int ProId { get; set; }

        public int WareId { get; set; }

        public double Quantity { get; set; } = 0;
        public double? Price { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProId))]
        public Product? Product { get; set; }
    }
}
