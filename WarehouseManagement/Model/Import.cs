using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;
using WarehouseManagement.Share;
using WarehouseManagement.Share.Enumeration;

namespace WarehouseManagement.Model
{
    public class Import : BaseEntity
    {
        public int EmployId { get; set; }
        [JsonIgnore]
        public DateTime CreateDate { get; set; } = DateTime.Now;
        public double Quantity { get; set; } = 0;
        public double TotalPrice { get; set; } = 0;
        public ImportEnum Status { get; set; }
        public string SupplierName { get; set; }
        public string? Tel { get; set; }
        public string Address { get; set; }
        public string? Email { get; set; }

        [JsonIgnore]
        [ForeignKey(nameof(EmployId))]
        public Employee? Employee { get; set; }
        public ICollection<ImportDetail>? ImportDetails { get; set; }
    }
}
