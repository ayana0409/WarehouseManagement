using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace WarehouseManagement.Model
{
    public class ImportDetail
    {
        public int ProId { get; set; }

        public int ImpId { get; set; }

        public double Quantity { get; set; } = 0;
        public double Price { get; set; } = 0;
        public DateTime ManuDate { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(ProId))]
        public Product? Product { get; set; }
    }
}
