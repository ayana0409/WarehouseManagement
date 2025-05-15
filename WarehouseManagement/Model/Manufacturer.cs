using WarehouseManagement.Share;

namespace WarehouseManagement.Model
{
    public class Manufacturer : BaseEntity
    {
        public string ManuName { get; set; }
        public string Address { get; set; }
        public string? Tel { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
