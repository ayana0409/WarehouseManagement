using System.Text.Json.Serialization;
using WarehouseManagement.Share;

namespace WarehouseManagement.Model
{
    public class TransferLog : BaseEntity
    {
        public int WhSourceId { get; set; }
        public int WhTargetId { get; set; }
        public DateTime? CreatedDate { get; set; }
        public string? Description { get; set; }
        public int? EmployeeId { get; set; }

    }
}
