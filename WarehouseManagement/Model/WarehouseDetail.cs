using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WarehouseManagement.Model
{
    public class WarehouseDetail
    {
        public int ProId { get; set; }

        public int WareId { get; set; }

        public double Quantity { get; set; } = 0;

        [JsonIgnore]
        [ForeignKey(nameof(ProId))]
        public Product? Product { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(WareId))]
        public Warehouse? Warehouse { get; set; }
    }

}
