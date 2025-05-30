using WarehouseManagement.Share;

namespace WarehouseManagement.Model
{
    public class Warehouse : BaseEntity
    {
        public string WareName { get; set; }
        public string Address { get; set; }
        public string Tel { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; } = true;

        public ICollection<WarehouseDetail>? WarehouseDetails { get; set; }
        //public ICollection<ExportDetail>? ExportDetails { get; set; }
    }

}
