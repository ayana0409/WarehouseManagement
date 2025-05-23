using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WarehouseManagement.Share;

namespace WarehouseManagement.Model
{
    public class Export : BaseEntity
    {
        public int EmployId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public double Quantity { get; set; } = 0;
        public double TotalPrice { get; set; } = 0;
        public string ConsumerName { get; set; }
        public string? Tel { get; set; }
        public string? Address { get; set; }

        [ForeignKey(nameof(EmployId))]
        public Employee? Employee { get; set; }
        [JsonIgnore]
        public ICollection<ExportDetail>? ExportDetails { get; set; }
    }
}
