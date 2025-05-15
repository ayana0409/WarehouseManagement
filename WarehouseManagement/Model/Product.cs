using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WarehouseManagement.Share;

namespace WarehouseManagement.Model
{
    public class Product : BaseEntity
    {
        public int ManuId { get; set; }
        public int CateId { get; set; }
        public string ProName { get; set; }
        public string? Image { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public double Quantity { get; set; } = 0;
        public string Unit { get; set; }
        public int Expiry { get; set; } = 0;
        public bool IsActive { get; set; } = true;

        [JsonIgnore]
        [ForeignKey(nameof(ManuId))]
        public Manufacturer? Manufacturer { get; set; }
        [JsonIgnore]
        [ForeignKey(nameof(CateId))]
        public Category? Category { get; set; }
    }

}
